using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Compartilhado;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MassTransit;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public record SolicitarGeracaoCertificadosCommand(
    Guid CursoId,
    IReadOnlyList<string> NomesAlunos
) : IRequest<Result<GeradorDto>>;

public sealed class SolicitarGeracaoCertificadosCommandHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioGerador repositorioGerador,
    IPublishEndpoint publishEndpoint
) : IRequestHandler<SolicitarGeracaoCertificadosCommand, Result<GeradorDto>>
{
    public async Task<Result<GeradorDto>> Handle(
        SolicitarGeracaoCertificadosCommand request,
        CancellationToken cancellationToken)
    {
        bool cursoExiste = await repositorioCurso.ExistePorIdAsync(
            request.CursoId,
            cancellationToken
        );

        if (!cursoExiste)
        {
            return Result.Fail(ErrosDeCurso.NaoEncontrado(request.CursoId));
        }

        bool emAndamento = await repositorioGerador.ExisteEmAndamentoPorCursoIdAsync(
            request.CursoId,
            cancellationToken
        );

        if (emAndamento)
        {
            return Result.Fail(ErrosCertificado.GeradorEmAndamento(request.CursoId));
        }

        Gerador gerador = new(
            Guid.CreateVersion7(),
            request.CursoId,
            request.NomesAlunos
        );

        IReadOnlyList<ErroValidacao> erros = gerador.Validar();

        if (erros.Count > 0)
        {
            return Result.Fail(ErrosCertificado.Validacao(erros));
        }

        await repositorioGerador.CadastrarAsync(gerador, cancellationToken);

        await publishEndpoint.Publish(
            new CertificadoMessage(gerador.Id, request.CursoId),
            cancellationToken
        );

        return Result.Ok(MapearGerador(gerador));
    }

    private static GeradorDto MapearGerador(Gerador gerador)
    {
        return new GeradorDto(
            gerador.Id,
            gerador.CursoId,
            gerador.Status,
            gerador.CaminhoZip,
            [.. gerador.Certificados.Select(certificado => new CertificadoDto(
                certificado.Id,
                certificado.NomeAluno,
                certificado.CaminhoArquivo,
                certificado.DataGeracao,
                certificado.Status
            ))]
        );
    }
}