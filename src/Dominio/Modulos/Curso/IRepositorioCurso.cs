using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Curso;

public interface IRepositorioCurso : IRepositorio<Curso>
{
    Task<bool> ExistePorIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken
    );
}