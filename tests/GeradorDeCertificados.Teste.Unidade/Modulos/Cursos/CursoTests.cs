
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorDeCertificados.Teste.Unidade.Modulos.Cursos;

[TestClass]
public class CursoTests
{
    private static Curso CriarCursoValido()
    {
        return new Curso(
            Guid.NewGuid(),
            "Programação C#",
            "Curso de desenvolvimento com .NET",
            40,
            new DateTime(2026, 9, 25)
        );
    }

    [TestMethod]
    public void Construtor_DevePreencherPropriedadesCorretamente()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        DateTime data = new(2026, 9, 25);

        // Act
        Curso curso = new(id, "Programação C#", "Descrição", 40, data);

        // Assert
        Assert.AreEqual(id, curso.Id);
        Assert.AreEqual("Programação C#", curso.Nome);
        Assert.AreEqual("Descrição", curso.Descricao);
        Assert.AreEqual(40, curso.CargaHoraria);
        Assert.AreEqual(data, curso.DataConclusao);
    }

    [TestMethod]
    public void Construtor_DeveRemoverEspacosDasPontasDoNome()
    {
        Curso curso = new(
            Guid.NewGuid(),
            "  Programação C#  ",
            null,
            40,
            DateTime.Today
        );

        Assert.AreEqual("Programação C#", curso.Nome);
    }

    [TestMethod]
    public void Validar_DeveAceitarCursoValido()
    {
        Curso curso = CriarCursoValido();

        var erros = curso.Validar();

        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void Validar_DeveRejeitarNomeVazio()
    {
        Curso curso = new(
            Guid.NewGuid(),
            "",
            null,
            40,
            DateTime.Today
        );

        var erros = curso.Validar();

        Assert.IsTrue(erros.Any(e => e.Campo == nameof(Curso.Nome)));
    }

    [TestMethod]
    public void Validar_DeveRejeitarNomeComMenosDeDoisCaracteres()
    {
        Curso curso = new(
            Guid.NewGuid(),
            "A",
            null,
            40,
            DateTime.Today
        );

        var erros = curso.Validar();

        Assert.IsTrue(erros.Any(e => e.Campo == nameof(Curso.Nome)));
    }

    [TestMethod]
    public void Validar_DeveRejeitarNomeComMaisDeCemCaracteres()
    {
        Curso curso = new(
            Guid.NewGuid(),
            new string('A', 101),
            null,
            40,
            DateTime.Today
        );

        var erros = curso.Validar();

        Assert.IsTrue(erros.Any(e => e.Campo == nameof(Curso.Nome)));
    }

    [TestMethod]
    public void Validar_DeveRejeitarDescricaoComMaisDeQuinhentosCaracteres()
    {
        Curso curso = new(
            Guid.NewGuid(),
            "Programação C#",
            new string('A', 501),
            40,
            DateTime.Today
        );

        var erros = curso.Validar();

        Assert.IsTrue(erros.Any(e => e.Campo == nameof(Curso.Descricao)));
    }

    [TestMethod]
    public void Validar_DeveAceitarDescricaoNula()
    {
        Curso curso = new(
            Guid.NewGuid(),
            "Programação C#",
            null,
            40,
            DateTime.Today
        );

        Assert.AreEqual(0, curso.Validar().Count);
    }

    [TestMethod]
    public void Validar_DeveRejeitarCargaHorariaZero()
    {
        Curso curso = new(
            Guid.NewGuid(),
            "Programação C#",
            null,
            0,
            DateTime.Today
        );

        var erros = curso.Validar();

        Assert.IsTrue(erros.Any(e => e.Campo == nameof(Curso.CargaHoraria)));
    }

    [TestMethod]
    public void Validar_DeveRejeitarCargaHorariaNegativa()
    {
        Curso curso = new(
            Guid.NewGuid(),
            "Programação C#",
            null,
            -10,
            DateTime.Today
        );

        var erros = curso.Validar();

        Assert.IsTrue(erros.Any(e => e.Campo == nameof(Curso.CargaHoraria)));
    }

    [TestMethod]
    public void Validar_DeveRejeitarDataConclusaoNaoPreenchida()
    {
        Curso curso = new(
            Guid.NewGuid(),
            "Programação C#",
            null,
            40,
            default
        );

        var erros = curso.Validar();

        Assert.IsTrue(erros.Any(e => e.Campo == nameof(Curso.DataConclusao)));
    }

    [TestMethod]
    public void Atualizar_DeveAtualizarTodasAsPropriedades()
    {
        Curso curso = CriarCursoValido();

        Curso cursoAtualizado = new(
            Guid.NewGuid(),
            "ASP.NET Core",
            "Nova descrição",
            80,
            new DateTime(2026, 12, 10)
        );

        curso.Atualizar(cursoAtualizado);

        Assert.AreEqual("ASP.NET Core", curso.Nome);
        Assert.AreEqual("Nova descrição", curso.Descricao);
        Assert.AreEqual(80, curso.CargaHoraria);
        Assert.AreEqual(new DateTime(2026, 12, 10), curso.DataConclusao);
    }
}
