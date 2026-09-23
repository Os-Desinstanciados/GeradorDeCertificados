using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public interface IRepositorioCertificado : IRepositorio<Certificado>
{
    Task<List<Certificado>> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );

    Stream Abrir(string caminho);

    Task<string> CompactarAsync(
        Guid cursoId,
        Guid geadorId,
        IReadOnlyList<string> caminhoPdf,
        CancellationToken cancellationToken
    );
}