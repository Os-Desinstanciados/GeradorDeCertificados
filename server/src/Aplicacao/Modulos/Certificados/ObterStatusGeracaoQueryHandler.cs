using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

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