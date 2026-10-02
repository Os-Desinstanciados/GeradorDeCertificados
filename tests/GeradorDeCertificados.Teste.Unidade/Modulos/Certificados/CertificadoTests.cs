
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorDeCertificados.Teste.Unidade.Modulos.Certificados;

[TestClass]
public class CertificadoTests
{
    private static Certificado CriarCertificado(string nome = "João")
    {
        return new Certificado(
            Guid.NewGuid(),
            Guid.NewGuid(),
            nome
        );
    }

    [TestMethod]
    public void Construtor_DeveIniciarCertificadoComoPendente()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid geradorId = Guid.NewGuid();
        string nomeAluno = "João";

        // Act
        Certificado certificado = new(id, geradorId, nomeAluno);

        // Assert
        Assert.AreEqual(id, certificado.Id);
        Assert.AreEqual(geradorId, certificado.GeradorId);
        Assert.AreEqual(nomeAluno, certificado.NomeAluno);
        Assert.AreEqual(StatusCertificado.Pendente, certificado.Status);
        Assert.IsNull(certificado.CaminhoArquivo);
        Assert.IsNull(certificado.DataGeracao);
    }

    [TestMethod]
    public void Construtor_DeveRemoverEspacosDoNome()
    {
        // Arrange
        string nomeAluno = "  João  ";

        // Act
        Certificado certificado = CriarCertificado(nomeAluno);

        // Assert
        Assert.AreEqual("João", certificado.NomeAluno);
    }

    [TestMethod]
    public void Validar_DeveAceitarNomeValido()
    {
        // Arrange
        Certificado certificado = CriarCertificado();

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void Validar_DeveRejeitarNomeVazio()
    {
        // Arrange
        Certificado certificado = CriarCertificado("");

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
    }

    [TestMethod]
    public void Validar_DeveRejeitarNomeComEspacos()
    {
        // Arrange
        Certificado certificado = CriarCertificado("   ");

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
    }

    [TestMethod]
    public void Validar_DeveRejeitarNomeComMaisDe50Caracteres()
    {
        // Arrange
        string nomeAluno = new('A', 51);
        Certificado certificado = CriarCertificado(nomeAluno);

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
    }

    [TestMethod]
    public void Validar_DeveAceitarNomeCom50Caracteres()
    {
        // Arrange
        string nomeAluno = new('A', 50);
        Certificado certificado = CriarCertificado(nomeAluno);

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void RegistrarGeracao_DeveAtualizarStatusCaminhoEData()
    {
        // Arrange
        Certificado certificado = CriarCertificado();
        string caminhoArquivo = "certificados/joao.pdf";

        // Act
        certificado.RegistrarGeracao(caminhoArquivo);

        // Assert
        Assert.AreEqual(StatusCertificado.Gerado, certificado.Status);
        Assert.AreEqual(caminhoArquivo, certificado.CaminhoArquivo);
        Assert.IsNotNull(certificado.DataGeracao);
    }

    [TestMethod]
    public void RegistrarGeracao_DeveRejeitarCaminhoVazio()
    {
        // Arrange
        Certificado certificado = CriarCertificado();

        // Act
        Action acao = () => certificado.RegistrarGeracao("");

        // Assert
        Assert.ThrowsExactly<ArgumentException>(acao);
    }

    [TestMethod]
    public void RegistrarGeracao_DeveRejeitarCaminhoComEspacos()
    {
        // Arrange
        Certificado certificado = CriarCertificado();

        // Act
        Action acao = () => certificado.RegistrarGeracao("   ");

        // Assert
        Assert.ThrowsExactly<ArgumentException>(acao);
    }

    [TestMethod]
    public void RegistrarFalha_DeveAtualizarStatus()
    {
        // Arrange
        Certificado certificado = CriarCertificado();

        // Act
        certificado.RegistrarFalha();

        // Assert
        Assert.AreEqual(StatusCertificado.Falha, certificado.Status);
    }

    [TestMethod]
    public void Atualizar_DeveCopiarPropriedades()
    {
        // Arrange
        Certificado original = CriarCertificado("João");
        Certificado atualizado = CriarCertificado("Maria");

        atualizado.RegistrarGeracao("certificados/maria.pdf");

        // Act
        original.Atualizar(atualizado);

        // Assert
        Assert.AreEqual("Maria", original.NomeAluno);
        Assert.AreEqual(StatusCertificado.Gerado, original.Status);
        Assert.AreEqual(
            "certificados/maria.pdf",
            original.CaminhoArquivo
        );
        Assert.IsNotNull(original.DataGeracao);
    }
}
