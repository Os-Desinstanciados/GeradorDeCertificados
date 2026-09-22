using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificado;

public interface IRepositorioCertificado : IRepositorio<Certificado>
{
    Task<List<Certificado>> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );
}