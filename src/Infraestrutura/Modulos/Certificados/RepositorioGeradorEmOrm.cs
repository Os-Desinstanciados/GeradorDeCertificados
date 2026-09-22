using GeradorDeCertificados.Dominio.Modulos.Certificado;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class RepositorioGeradorEmOrm(
    GeradorDeCertificadosDbContext dbContext
) : RepositorioBaseEmOrm<Gerador>(dbContext), IRepositorioGerador
{
    public override async Task<Gerador?> SelecionarPorIdAsync(
        Guid idSelecionado,
        CancellationToken cancellationToken = default
    )
    {
        return await registros
            .Include(g => g.Certificados)
            .SingleOrDefaultAsync(g => g.Id == idSelecionado, cancellationToken);
    }

    public async Task<Gerador?> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return await SelecionarMaisRecentePorCursoIdAsync(cursoId, cancellationToken);
    }

    public async Task<bool> ExisteEmAndamentoPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return await registros.AnyAsync(
            g => g.CursoId == cursoId &&
                 (g.Status == StatusGerador.Pendente ||
                  g.Status == StatusGerador.GerandoCertificados ||
                  g.Status == StatusGerador.GerandoZip),
            cancellationToken
        );
    }

    public async Task<Gerador?> SelecionarMaisRecentePorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return await registros
            .Include(g => g.Certificados)
            .Where(g => g.CursoId == cursoId)
            .OrderByDescending(g => g.DataSolicitacao)
            .FirstOrDefaultAsync(cancellationToken);
    }
}