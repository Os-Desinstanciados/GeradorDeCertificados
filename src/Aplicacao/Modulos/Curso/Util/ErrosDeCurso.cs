using GeradorDeCertificados.Dominio.Compartilhado;
using FluentResults;
using GeradorDeCertificados.Aplicacao.Compartilhado;

namespace GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;

public static class ErrosDeCurso
{
    public static Error NaoEncontrado(Guid idCurso)
    {
        return new Error("O curso com este ID não foi encontrado.")
            .WithMetadata(nameof(TipoErro), TipoErro.NaoEncontrado)
            .WithMetadata("IdCurso", idCurso);
    }

    public static IEnumerable<Error> Validacao(
        IEnumerable<ErroValidacao> erros)
    {
        return erros.Select(erro => new Error(erro.Mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", erro.Campo));
    }
}