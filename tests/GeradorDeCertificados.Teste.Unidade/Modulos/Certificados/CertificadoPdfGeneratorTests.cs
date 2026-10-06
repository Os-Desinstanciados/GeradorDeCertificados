using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Infraestrutura.Modulos.Certificados;
using Microsoft.Extensions.Options;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace GeradorDeCertificados.Teste.Unidade.Modulos.Certificados;

[TestClass]
public sealed class CertificadoPdfGeneratorTests
{
    private string diretorioTemporario = null!;

    [TestInitialize]
    public void Configurar()
    {
        diretorioTemporario = Path.Combine(
            Path.GetTempPath(),
            $"GeradorDeCertificados-{Guid.NewGuid()}"
        );
    }

    [TestCleanup]
    public void Limpar()
    {
        if (Directory.Exists(diretorioTemporario))
            Directory.Delete(diretorioTemporario, true);
    }

    [TestMethod]
    public async Task Gerar_Certificado_ContemDadosDoAlunoEDoCurso()
    {
        // Arrange
        CertificadoPdfGenerator gerador = CriarGerador();

        Curso curso = CriarCurso();

        Certificado certificado = new(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            "João da Silva"
        );

        // Act
        string caminhoPdf = await gerador.GerarAsync(
            certificado,
            curso,
            CancellationToken.None
        );

        string texto = LerTexto(caminhoPdf);

        // Assert
        Assert.IsTrue(File.Exists(caminhoPdf));
        Assert.Contains("João da Silva", texto);
        Assert.Contains("Desenvolvimento de Sistemas", texto);
        Assert.Contains("40 horas", texto);
        Assert.Contains("05/10/2026", texto);
    }

    [TestMethod]
    public async Task Gerar_Certificado_CriaPdfComUmaPagina()
    {
        // Arrange
        CertificadoPdfGenerator gerador = CriarGerador();

        Curso curso = CriarCurso();

        Certificado certificado = new(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            "Maria Oliveira"
        );

        // Act
        string caminhoPdf = await gerador.GerarAsync(
            certificado,
            curso,
            CancellationToken.None
        );

        using PdfDocument documento = PdfDocument.Open(caminhoPdf);

        // Assert
        Assert.AreEqual(1, documento.NumberOfPages);
    }

    private CertificadoPdfGenerator CriarGerador()
    {
        CertificadosArquivosOptions configuracao = new()
        {
            DiretorioArquivos = diretorioTemporario
        };

        return new CertificadoPdfGenerator(
            Options.Create(configuracao)
        );
    }

    private static Curso CriarCurso()
    {
        return new Curso(
            Guid.CreateVersion7(),
            "Desenvolvimento de Sistemas",
            "Curso de desenvolvimento",
            40,
            new DateTime(2026, 10, 5)
        );
    }

    private static string LerTexto(string caminhoPdf)
    {
        using PdfDocument documento = PdfDocument.Open(caminhoPdf);

        return string.Join(
            Environment.NewLine,
            documento.GetPages()
                .Select(pagina =>
                    ContentOrderTextExtractor.GetText(pagina, true))
        );
    }
}