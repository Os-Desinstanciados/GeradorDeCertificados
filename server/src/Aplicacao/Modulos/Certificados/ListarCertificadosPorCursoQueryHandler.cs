using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public record ListarCertificadosPorCursoQuery(Guid CursoId)
    : IRequest<Result<IReadOnlyList<CertificadoDto>>>;

public sealed class ListarCertificadosPorCursoQueryHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioGerador repositorioGerador
) : IRequestHandler<ListarCertificadosPorCursoQuery, Result<IReadOnlyList<CertificadoDto>>>
{
    public async Task<Result<IReadOnlyList<CertificadoDto>>> Handle(
        ListarCertificadosPorCursoQuery query,
        CancellationToken cancellationToken)
    {
        bool cursoExiste = await repositorioCurso.ExistePorIdAsync(
            query.CursoId,
            cancellationToken
        );

        if (!cursoExiste)
        {
            return Result.Fail(ErrosDeCurso.NaoEncontrado(query.CursoId));
        }

        Gerador? gerador =
            await repositorioGerador.SelecionarPorCursoIdAsync(
                query.CursoId,
                cancellationToken
            );

        if (gerador is null)
        {
            return Result.Ok<IReadOnlyList<CertificadoDto>>([]);
        }

        IReadOnlyList<CertificadoDto> certificados = [.. gerador.Certificados.Select(
            certificado => new CertificadoDto(
                certificado.Id,
                certificado.NomeAluno,
                certificado.CaminhoArquivo,
                certificado.DataGeracao,
                certificado.Status
            )
        )];

        return Result.Ok(certificados);
    }
}