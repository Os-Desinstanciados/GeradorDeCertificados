using GeradorDeCertificados.Dominio.Compartilhado;
using GeradorDeCertificados.Dominio.Modulos.Cursos;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

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

    Task<string> GerarAsync(
        Certificado certificado,
        Curso curso,
        CancellationToken cancellationToken
    );
}