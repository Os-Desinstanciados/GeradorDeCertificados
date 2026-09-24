using System.IO.Compression;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class RepositorioCertificadoEmOrm : RepositorioBaseEmOrm<Certificado>, IRepositorioCertificado
{
    private const string ApplicationDirectoryName = "GeradorCertificadosOnline";
    private const string StorageDirectoryName = "storage";
    private readonly GeradorDeCertificadosDbContext contexto;

    public RepositorioCertificadoEmOrm(GeradorDeCertificadosDbContext dbContext) : base(dbContext)
    {
        contexto = dbContext;
    }

    public async Task<List<Certificado>> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        Guid? geradorId = await contexto.Set<Gerador>()
            .Where(g => g.CursoId == cursoId)
            .OrderByDescending(g => g.DataSolicitacao)
            .Select(g => (Guid?)g.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (geradorId is null)
        {
            return [];
        }

        return await registros
            .Where(g => g.GeradorId == geradorId)
            .ToListAsync(cancellationToken);
    }

    public Stream Abrir(string caminho)
    {
        return File.OpenRead(caminho);
    }

    public async Task<string> CompactarAsync(
        Guid cursoId,
        Guid processamentoId,
        IReadOnlyList<string> caminhosPdf,
        CancellationToken cancellationToken)
    {
        var pastaCurso = ObterPastaCurso(cursoId);

        // O caminho absoluto é persistido para que a API possa abrir o arquivo depois.
        var caminhoArquivo = Path.GetFullPath(
            Path.Combine(pastaCurso, $"certificados-{processamentoId}.zip"));

        await using var stream = new MemoryStream();

        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var caminho in caminhosPdf)
                archive.CreateEntryFromFile(caminho, Path.GetFileName(caminho));
        }

        await SalvarBytesAsync(caminhoArquivo, stream.ToArray(), cancellationToken);

        return caminhoArquivo;
    }

    private string ObterPastaCurso(Guid cursoId)
    {
        var pastaCurso = Path.Combine(ObterCaminhoPadrao(), cursoId.ToString());
        Directory.CreateDirectory(pastaCurso);

        return pastaCurso;
    }

    private static string ObterCaminhoPadrao()
    {
        var localApplicationData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData,
            Environment.SpecialFolderOption.Create);

        if (string.IsNullOrWhiteSpace(localApplicationData))
            throw new InvalidOperationException(
                "Não foi possível determinar o diretório LocalApplicationData do usuário.");

        return Path.Combine(
            localApplicationData,
            ApplicationDirectoryName,
            StorageDirectoryName);
    }

    private static Task SalvarBytesAsync(
        string caminhoArquivo,
        byte[] conteudo,
        CancellationToken cancellationToken)
    {
        return File.WriteAllBytesAsync(caminhoArquivo, conteudo, cancellationToken);
    }

}