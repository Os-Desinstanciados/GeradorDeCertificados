using GeradorDeCertificados.Aplicacao.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MassTransit;
using Moq;

namespace GeradorDeCertificados.Teste.Unidade.Aplicacao.Certificados;

[TestClass]
public sealed class GerarCertificadosCommandHandlerTests
{
    [TestMethod]
    public async Task SolicitarGeracao_CursoNaoExiste_RetornaFalha()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        Mock<IRepositorioCurso> repositorioCurso = new();
        Mock<IRepositorioGerador> repositorioGerador = new();
        Mock<IPublishEndpoint> publishEndpoint = new();

        repositorioCurso
            .Setup(x => x.ExistePorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        SolicitarGeracaoCertificadosCommandHandler handler = new(
            repositorioCurso.Object,
            repositorioGerador.Object,
            publishEndpoint.Object
        );

        SolicitarGeracaoCertificadosCommand command = new(
            cursoId,
            ["João da Silva"]
        );

        // Act
        var resultado = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioGerador.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Gerador>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task SolicitarGeracao_GeracaoEmAndamento_RetornaFalha()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        Mock<IRepositorioCurso> repositorioCurso = new();
        Mock<IRepositorioGerador> repositorioGerador = new();
        Mock<IPublishEndpoint> publishEndpoint = new();

        repositorioCurso
            .Setup(x => x.ExistePorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(true);

        repositorioGerador
            .Setup(x => x.ExisteEmAndamentoPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(true);

        SolicitarGeracaoCertificadosCommandHandler handler = new(
            repositorioCurso.Object,
            repositorioGerador.Object,
            publishEndpoint.Object
        );

        SolicitarGeracaoCertificadosCommand command = new(
            cursoId,
            ["João da Silva"]
        );

        // Act
        var resultado = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioGerador.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Gerador>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task SolicitarGeracao_DadosValidos_CadastraGerador()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        Mock<IRepositorioCurso> repositorioCurso = new();
        Mock<IRepositorioGerador> repositorioGerador = new();
        Mock<IPublishEndpoint> publishEndpoint = new();

        repositorioCurso
            .Setup(x => x.ExistePorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(true);

        repositorioGerador
            .Setup(x => x.ExisteEmAndamentoPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        SolicitarGeracaoCertificadosCommandHandler handler = new(
            repositorioCurso.Object,
            repositorioGerador.Object,
            publishEndpoint.Object
        );

        SolicitarGeracaoCertificadosCommand command = new(
            cursoId,
            ["João da Silva"]
        );

        // Act
        var resultado = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        repositorioGerador.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Gerador>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}