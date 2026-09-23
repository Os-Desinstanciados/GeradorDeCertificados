using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificado.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificado.Util;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificado;
using GeradorDeCertificados.Dominio.Modulos.Curso;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificado;

public record ObterStatusGeracaoQuery(Guid CursoId)
    : IRequest<Result<StatusGeracaoDto>>;

public sealed class ObterStatusGeracaoQueryHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioGerador repositorioGerador
) : IRequestHandler<ObterStatusGeracaoQuery, Result<StatusGeracaoDto>>
{
    public async Task<Result<StatusGeracaoDto>> Handle(
        ObterStatusGeracaoQuery query,
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
            return Result.Fail(ErrosCertificado.GeracaoNaoEncontrada(query.CursoId));
        }

        return Result.Ok(new StatusGeracaoDto(
            gerador.Id,
            gerador.CursoId,
            gerador.Status
        ));
    }
}