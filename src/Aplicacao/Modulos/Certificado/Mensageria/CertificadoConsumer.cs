using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;

public sealed class CertificadoConsumer(
    IRepositorioGerador repositorioGerador,
    IRepositorioCurso repositorioCurso,    
    IRepositorioCertificado repositorioCertificado,
    ILogger<CertificadoConsumer> logger
) : IConsumer<CertificadoMessage>
{
    public async Task Consume(ConsumeContext<CertificadoMessage> context)
    {
        Gerador? gerador = await repositorioGerador.SelecionarPorIdAsync(
            context.Message.GeracaoId,
            context.CancellationToken
        );

        if (gerador is null)
        {
            logger.LogWarning(
                "Geradoção {GeracaoId} não encontrada.",
                context.Message.GeracaoId
            );
            return;
        }

        if (gerador.EstaFinalizado)
        {
            return;
        }

        if (gerador.Status == StatusGerador.Pendente)
        {
            gerador.GerarCertificados();
            await repositorioGerador.SalvarAsync(gerador, context.CancellationToken);
        }

        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(
            context.Message.CursoId,
            context.CancellationToken
        );

        if (curso is null)
        {
            logger.LogWarning(
                "Curso {CursoId} não encontrado para o processamento {ProcessamentoId}.",
                context.Message.CursoId,
                context.Message.ProcessamentoId
            );

            processamento.RegistrarFalhaProcessamento();
            await repositorioProcessamento.SalvarAsync(processamento, context.CancellationToken);
            return;
        }

        foreach (Certificado certificado in processamento.Certificados
            .Where(c => c.StatusGeracao == StatusGeracao.Pendente)
            .ToList())
        {
            try
            {
                string caminho = await pdfGenerator.GerarAsync(
                    certificado,
                    curso,
                    context.CancellationToken
                );

                processamento.RegistrarSucesso(certificado.Id, caminho);
                await repositorioProcessamento.SalvarAsync(processamento, context.CancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Falha ao gerar certificado {CertificadoId} do processamento {ProcessamentoId}.",
                    certificado.Id,
                    processamento.Id
                );

                processamento.RegistrarFalha(certificado.Id);
                await repositorioProcessamento.SalvarAsync(processamento, context.CancellationToken);
            }
        }

        if (!processamento.TodosCertificadosProcessados)
        {
            return;
        }

        if (processamento.Gerados == 0)
        {
            processamento.RegistrarFalhaProcessamento();
            await repositorioProcessamento.SalvarAsync(processamento, context.CancellationToken);
            return;
        }

        if (processamento.Status == StatusProcessamento.GerandoCertificados)
        {
            processamento.IniciarGeracaoZip();
            await repositorioProcessamento.SalvarAsync(processamento, context.CancellationToken);
        }

        if (processamento.CaminhoZip is not null)
        {
            return;
        }

        IReadOnlyList<string> caminhosPdf = [.. processamento.Certificados
            .Where(c => c.StatusGeracao == StatusGeracao.Gerado && !string.IsNullOrWhiteSpace(c.CaminhoArquivo))
            .Select(c => c.CaminhoArquivo!)];

        try
        {
            string caminhoZip = await certificadoStorage.CompactarAsync(
                processamento.CursoId,
                processamento.Id,
                caminhosPdf,
                context.CancellationToken
            );

            processamento.RegistrarZip(caminhoZip);
            await repositorioProcessamento.SalvarAsync(processamento, context.CancellationToken);

            logger.LogInformation(
                "Processamento {ProcessamentoId} concluído. Gerados: {Gerados}. Falhas: {Falhas}.",
                processamento.Id,
                processamento.Gerados,
                processamento.Falhas
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Falha ao compactar os certificados do processamento {ProcessamentoId}.",
                processamento.Id
            );

            processamento.RegistrarFalhaProcessamento();
            await repositorioProcessamento.SalvarAsync(processamento, context.CancellationToken);
        }
    }
}