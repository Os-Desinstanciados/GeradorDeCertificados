using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public record ObterArquivoCertificadoQuery(Guid CursoId)
    : IRequest<Result<ArquivoCertificadoDto>>;

public sealed class ObterArquivoCertificadoQueryHandler(
    IRepositorioGerador repositorioGerador,
    IRepositorioCertificado repositorioCertificado
) : IRequestHandler<ObterArquivoCertificadoQuery, Result<ArquivoCertificadoDto>>
{
    public async Task<Result<ArquivoCertificadoDto>> Handle(
        ObterArquivoCertificadoQuery query,
        CancellationToken cancellationToken)
    {
        Gerador? gerador =
            await repositorioGerador.SelecionarPorCursoIdAsync(
                query.CursoId,
                cancellationToken
            );

        if (gerador is null)
        {
            return Result.Fail(ErrosCertificado.GeracaoNaoEncontrada(query.CursoId));
        }

        if (!gerador.EstaFinalizado || gerador.CaminhoZip is null)
        {
            return Result.Fail(ErrosCertificado.ZipIndisponivel(query.CursoId));
        }

        return Result.Ok(new ArquivoCertificadoDto(
            repositorioCertificado.Abrir(gerador.CaminhoZip),
            $"certificados-{query.CursoId}.zip",
            "application/zip"
        ));
    }
}