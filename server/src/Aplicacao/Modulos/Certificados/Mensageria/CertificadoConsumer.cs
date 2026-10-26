using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;

public sealed class CertificadoConsumer(
    IRepositorioGerador repositorioGerador,
    IRepositorioCertificado repositorioCertificado,
    IRepositorioCurso repositorioCurso,        
    ICertificadoPdfGenerator pdfGenerator,
    ILogger<CertificadoConsumer> logger
) : IConsumer<CertificadoMessage>
{
    public async Task Consume(ConsumeContext<CertificadoMessage> context)
    {
        Gerador? gerador = await repositorioGerador.SelecionarPorIdAsync(
            context.Message.GeradorId,
            context.CancellationToken
        );

        if (gerador is null)
        {
            logger.LogWarning(
                "Geração {GeradorId} não encontrada.",
                context.Message.GeradorId
            );
            return;
        }

        if (gerador.EstaFinalizado)
        {
            return;
        }

        if (gerador.Status == StatusGerador.Pendente)
        {
            gerador.IniciarGeracaoCertificados();
            await repositorioGerador.SalvarAsync(gerador, context.CancellationToken);
        }

        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(
            context.Message.CursoId,
            context.CancellationToken
        );

        if (curso is null)
        {
            logger.LogWarning(
                "Curso {CursoId} não encontrado para o gerador {GeradorId}.",
                context.Message.CursoId,
                context.Message.GeradorId
            );

            gerador.RegistrarFalhaGerador();
            await repositorioGerador.SalvarAsync(gerador, context.CancellationToken);
            return;
        }

        foreach (var certificado in gerador.Certificados
            .Where(c => c.Status == StatusCertificado.Pendente)
            .ToList())
        {
            try
            {
                string caminho = await pdfGenerator.GerarAsync(
                    certificado,
                    curso,
                    context.CancellationToken
                );

                gerador.RegistrarSucesso(certificado.Id, caminho);
                await repositorioGerador.SalvarAsync(gerador, context.CancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Falha ao gerar certificado {CertificadoId} do gerador {GeradorId}.",
                    certificado.Id,
                    gerador.Id
                );

                gerador.RegistrarFalha(certificado.Id);
                await repositorioGerador.SalvarAsync(gerador, context.CancellationToken);
            }
        }

        if (!gerador.TodosCertificadosProcessados)
        {
            return;
        }

        if (gerador.Gerados == 0)
        {
            gerador.RegistrarFalhaGerador();
            await repositorioGerador.SalvarAsync(gerador, context.CancellationToken);
            return;
        }

        if (gerador.Status == StatusGerador.GerandoCertificados)
        {
            gerador.IniciarGeracaoZip();
            await repositorioGerador.SalvarAsync(gerador, context.CancellationToken);
        }

        if (gerador.CaminhoZip is not null)
        {
            return;
        }

        IReadOnlyList<string> caminhosPdf = [.. gerador.Certificados
            .Where(c => c.Status == StatusCertificado.Gerado && !string.IsNullOrWhiteSpace(c.CaminhoArquivo))
            .Select(c => c.CaminhoArquivo!)];

        try
        {
            string caminhoZip = await repositorioCertificado.CompactarAsync(
                gerador.CursoId,
                gerador.Id,
                caminhosPdf,
                context.CancellationToken
            );

            gerador.RegistrarZip(caminhoZip);
            await repositorioGerador.SalvarAsync(gerador, context.CancellationToken);

            logger.LogInformation(
                "Gerador {GeradorId} concluído. Gerados: {Gerados}. Falhas: {Falhas}.",
                gerador.Id,
                gerador.Gerados,
                gerador.Falhas
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Falha ao compactar os certificados do gerador {GeradorId}.",
                gerador.Id
            );

            gerador.RegistrarFalhaGerador();
            await repositorioGerador.SalvarAsync(gerador, context.CancellationToken);
        }
    }
}