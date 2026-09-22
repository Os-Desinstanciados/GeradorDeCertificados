using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificado;

public interface IRepositorioGerador : IRepositorio<Gerador>
{
    Task<Gerador?> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExisteEmAndamentoPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );

    Task<Gerador?> SelecionarMaisRecentePorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );
}