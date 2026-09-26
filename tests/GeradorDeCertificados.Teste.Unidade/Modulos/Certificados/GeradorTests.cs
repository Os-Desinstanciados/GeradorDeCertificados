
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorDeCertificados.Teste.Unidade.Modulos.Certificados;

[TestClass]
public class GeradorTests
{
    private static Gerador CriarGerador(params string[] alunos)
    {
        return new Gerador(
            Guid.NewGuid(),
            Guid.NewGuid(),
            alunos.Length > 0 ? alunos : ["João"]
        );
    }

    private static void GerarTodosCertificados(Gerador gerador)
    {
        foreach (Certificado certificado in gerador.Certificados)
        {
            gerador.RegistrarSucesso(
                certificado.Id,
                $"{certificado.Id}.pdf"
            );
        }
    }

    [TestMethod]
    public void Construtor_DeveIniciarComoPendente()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid cursoId = Guid.NewGuid();
        string[] alunos = ["João", "Maria"];

        // Act
        Gerador gerador = new(id, cursoId, alunos);

        // Assert
        Assert.AreEqual(id, gerador.Id);
        Assert.AreEqual(cursoId, gerador.CursoId);
        Assert.AreEqual(StatusGerador.Pendente, gerador.Status);
        Assert.AreEqual(2, gerador.Certificados.Count);
        Assert.AreEqual(0, gerador.Gerados);
        Assert.AreEqual(0, gerador.Falhas);
        Assert.IsTrue(gerador.EstaEmAndamento);
        Assert.IsFalse(gerador.EstaFinalizado);
        Assert.IsFalse(gerador.TodosCertificadosProcessados);
        Assert.IsNull(gerador.CaminhoZip);
        Assert.IsNull(gerador.ConcluidoEm);
    }

    [TestMethod]
    public void Validar_DeveAceitarGeradorValido()
    {
        // Arrange
        Gerador gerador = CriarGerador("João", "Maria");

        // Act
        var erros = gerador.Validar();

        // Assert
        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void Validar_DeveRejeitarGeradorSemAlunos()
    {
        // Arrange
        Gerador gerador = new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            []
        );

        // Act
        var erros = gerador.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e => e.Campo == "Alunos"));
    }

    [TestMethod]
    public void Validar_DeveRejeitarAlunoComNomeInvalido()
    {
        // Arrange
        Gerador gerador = CriarGerador("João", "");

        // Act
        var erros = gerador.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Certificado.NomeAluno)
        ));
    }

    [TestMethod]
    public void IniciarGeracaoCertificados_DeveAlterarStatus()
    {
        // Arrange
        Gerador gerador = CriarGerador();

        // Act
        gerador.IniciarGeracaoCertificados();

        // Assert
        Assert.AreEqual(
            StatusGerador.GerandoCertificados,
            gerador.Status
        );
        Assert.IsTrue(gerador.EstaEmAndamento);
    }

    [TestMethod]
    public void IniciarGeracaoCertificados_DeveRejeitarSegundaInicializacao()
    {
        // Arrange
        Gerador gerador = CriarGerador();
        gerador.IniciarGeracaoCertificados();

        // Act
        Action acao = () => gerador.IniciarGeracaoCertificados();

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(acao);
    }

    [TestMethod]
    public void RegistrarSucesso_DeveAtualizarCertificadoEContador()
    {
        // Arrange
        Gerador gerador = CriarGerador("João", "Maria");
        Certificado certificado = gerador.Certificados.First();

        // Act
        gerador.RegistrarSucesso(certificado.Id, "joao.pdf");

        // Assert
        Assert.AreEqual(StatusCertificado.Gerado, certificado.Status);
        Assert.AreEqual("joao.pdf", certificado.CaminhoArquivo);
        Assert.AreEqual(1, gerador.Gerados);
        Assert.AreEqual(0, gerador.Falhas);
        Assert.IsFalse(gerador.TodosCertificadosProcessados);
    }

    [TestMethod]
    public void RegistrarFalha_DeveAtualizarCertificadoEContador()
    {
        // Arrange
        Gerador gerador = CriarGerador("João", "Maria");
        Certificado certificado = gerador.Certificados.First();

        // Act
        gerador.RegistrarFalha(certificado.Id);

        // Assert
        Assert.AreEqual(StatusCertificado.Falha, certificado.Status);
        Assert.AreEqual(1, gerador.Falhas);
        Assert.AreEqual(0, gerador.Gerados);
        Assert.IsFalse(gerador.TodosCertificadosProcessados);
    }

    [TestMethod]
    public void RegistrarSucesso_DeveRejeitarCertificadoJaProcessado()
    {
        // Arrange
        Gerador gerador = CriarGerador();
        Certificado certificado = gerador.Certificados.Single();

        gerador.RegistrarSucesso(certificado.Id, "joao.pdf");

        // Act
        Action acao = () =>
            gerador.RegistrarSucesso(certificado.Id, "outro.pdf");

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(acao);
    }

    [TestMethod]
    public void TodosCertificadosProcessados_DeveSerVerdadeiroAposProcessamento()
    {
        // Arrange
        Gerador gerador = CriarGerador("João", "Maria");
        Certificado[] certificados = gerador.Certificados.ToArray();

        // Act
        gerador.RegistrarSucesso(certificados[0].Id, "joao.pdf");
        gerador.RegistrarFalha(certificados[1].Id);

        // Assert
        Assert.IsTrue(gerador.TodosCertificadosProcessados);
        Assert.AreEqual(1, gerador.Gerados);
        Assert.AreEqual(1, gerador.Falhas);
    }

    [TestMethod]
    public void IniciarGeracaoZip_DeveRejeitarStatusPendente()
    {
        // Arrange
        Gerador gerador = CriarGerador();

        // Act
        Action acao = () => gerador.IniciarGeracaoZip();

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(acao);
    }

    [TestMethod]
    public void IniciarGeracaoZip_DeveRejeitarCertificadosPendentes()
    {
        // Arrange
        Gerador gerador = CriarGerador("João", "Maria");
        gerador.IniciarGeracaoCertificados();

        Certificado certificado = gerador.Certificados.First();
        gerador.RegistrarSucesso(certificado.Id, "joao.pdf");

        // Act
        Action acao = () => gerador.IniciarGeracaoZip();

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(acao);
    }

    [TestMethod]
    public void IniciarGeracaoZip_DeveRejeitarQuandoTodosFalharam()
    {
        // Arrange
        Gerador gerador = CriarGerador();
        gerador.IniciarGeracaoCertificados();

        Certificado certificado = gerador.Certificados.Single();
        gerador.RegistrarFalha(certificado.Id);

        // Act
        Action acao = () => gerador.IniciarGeracaoZip();

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(acao);
    }

    [TestMethod]
    public void IniciarGeracaoZip_DeveAceitarCertificadosProcessados()
    {
        // Arrange
        Gerador gerador = CriarGerador("João", "Maria");
        gerador.IniciarGeracaoCertificados();
        GerarTodosCertificados(gerador);

        // Act
        gerador.IniciarGeracaoZip();

        // Assert
        Assert.AreEqual(StatusGerador.GerandoZip, gerador.Status);
        Assert.IsTrue(gerador.EstaEmAndamento);
    }

    [TestMethod]
    public void RegistrarZip_DeveConcluirGerador()
    {
        // Arrange
        Gerador gerador = CriarGerador("João", "Maria");
        gerador.IniciarGeracaoCertificados();
        GerarTodosCertificados(gerador);
        gerador.IniciarGeracaoZip();

        // Act
        gerador.RegistrarZip("certificados.zip");

        // Assert
        Assert.AreEqual(StatusGerador.Concluido, gerador.Status);
        Assert.AreEqual("certificados.zip", gerador.CaminhoZip);
        Assert.IsNotNull(gerador.ConcluidoEm);
        Assert.IsTrue(gerador.EstaFinalizado);
        Assert.IsFalse(gerador.EstaEmAndamento);
    }

    [TestMethod]
    public void RegistrarZip_DeveRejeitarCaminhoVazio()
    {
        // Arrange
        Gerador gerador = CriarGerador();
        gerador.IniciarGeracaoCertificados();
        GerarTodosCertificados(gerador);
        gerador.IniciarGeracaoZip();

        // Act
        Action acao = () => gerador.RegistrarZip("");

        // Assert
        Assert.ThrowsExactly<ArgumentException>(acao);
        Assert.AreEqual(StatusGerador.GerandoZip, gerador.Status);
    }

    [TestMethod]
    public void RegistrarZip_DeveRejeitarStatusIncorreto()
    {
        // Arrange
        Gerador gerador = CriarGerador();

        // Act
        Action acao = () => gerador.RegistrarZip("certificados.zip");

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(acao);
    }

    [TestMethod]
    public void RegistrarFalhaGerador_DeveFinalizarComFalha()
    {
        // Arrange
        Gerador gerador = CriarGerador();

        // Act
        gerador.RegistrarFalhaGerador();

        // Assert
        Assert.AreEqual(StatusGerador.Falha, gerador.Status);
        Assert.IsNotNull(gerador.ConcluidoEm);
        Assert.IsTrue(gerador.EstaFinalizado);
        Assert.IsFalse(gerador.EstaEmAndamento);
    }

    [TestMethod]
    public void RegistrarFalhaGerador_DeveRejeitarGeradorFinalizado()
    {
        // Arrange
        Gerador gerador = CriarGerador();
        gerador.RegistrarFalhaGerador();

        // Act
        Action acao = () => gerador.RegistrarFalhaGerador();

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(acao);
    }

    [TestMethod]
    public void Atualizar_DeveCopiarStatusECaminhoZip()
    {
        // Arrange
        Gerador original = CriarGerador();

        Gerador atualizado = CriarGerador();
        atualizado.IniciarGeracaoCertificados();
        GerarTodosCertificados(atualizado);
        atualizado.IniciarGeracaoZip();
        atualizado.RegistrarZip("certificados.zip");

        // Act
        original.Atualizar(atualizado);

        // Assert
        Assert.AreEqual(StatusGerador.Concluido, original.Status);
        Assert.AreEqual("certificados.zip", original.CaminhoZip);
    }
}
