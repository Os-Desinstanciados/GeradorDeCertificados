using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class RepositorioCertificadoEmOrm : RepositorioBaseEmOrm<Certificado>, IRepositorioCertificado
{
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
}