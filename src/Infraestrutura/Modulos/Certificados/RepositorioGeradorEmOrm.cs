using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class RepositorioGeradorEmOrm
    : RepositorioBaseEmOrm<Gerador>, IRepositorioGerador
{
    private readonly GeradorDeCertificadosDbContext dbContext;

    public RepositorioGeradorEmOrm(GeradorDeCertificadosDbContext dbContext)
        : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task SalvarAsync(
        Gerador gerador,
        CancellationToken cancellationToken = default
    )
    {
        dbContext.ChangeTracker.Clear();
        dbContext.Attach(gerador);
        dbContext.Entry(gerador).State = EntityState.Modified;

        foreach (Certificado certificado in gerador.Certificados)
        {
            dbContext.Entry(certificado).State = EntityState.Modified;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        dbContext.ChangeTracker.Clear();
    }

    public override async Task<Gerador?> SelecionarPorIdAsync(
        Guid idSelecionado,
        CancellationToken cancellationToken = default
    )
    {
        return await registros
            .AsNoTracking()
            .Include(p => p.Certificados)
            .SingleOrDefaultAsync(p => p.Id == idSelecionado, cancellationToken);
    }

    public async Task<Gerador?> SelecionarPorCursoAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return await registros
            .AsNoTracking()
            .Include(p => p.Certificados)
            .Where(p => p.CursoId == cursoId)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Gerador?> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return SelecionarPorCursoAsync(cursoId, cancellationToken);
    }

    public async Task<bool> ExisteEmAndamentoPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return await registros.AnyAsync(
            p => p.CursoId == cursoId
                && (p.Status == StatusGerador.Pendente
                    || p.Status == StatusGerador.GerandoCertificados
                    || p.Status == StatusGerador.GerandoZip),
            cancellationToken
        );
    }
}