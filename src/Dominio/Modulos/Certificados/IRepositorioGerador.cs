using GeradorDeCertificados.Dominio.Compartilhado;
using GeradorDeCertificados.Dominio.Modulos.Cursos;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public interface IRepositorioGerador : IRepositorio<Gerador>
{
    Task SalvarAsync(
        Gerador gerador,
        CancellationToken cancellationToken = default
    );

    Task<Gerador?> SelecionarPorCursoAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );

    Task<Gerador?> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExisteEmAndamentoPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );
}