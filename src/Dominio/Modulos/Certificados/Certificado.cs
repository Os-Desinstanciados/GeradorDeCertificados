using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public sealed class Certificado : EntidadeBase<Certificado>
{
    public Guid GeradorId { get; private set; }
    public string NomeAluno { get; private set; } = string.Empty;
    public string? CaminhoArquivo { get; private set; }
    public DateTime? DataGeracao { get; private set; }
    public StatusCertificado Status { get; private set; }

    private Certificado() { }

    public Certificado(Guid id, Guid geradorId, string nomeAluno)
    {
        Id = id;
        GeradorId = geradorId;
        NomeAluno = nomeAluno?.Trim() ?? string.Empty;
        Status = StatusCertificado.Pendente;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(NomeAluno))
        {
            erros.Add(new ErroValidacao(
                nameof(NomeAluno),
                "O campo \"Nome do Aluno\" é obrigatório."
            ));
        }
        else if (NomeAluno.Length > 50)
        {
            erros.Add(new ErroValidacao(
                nameof(NomeAluno),
                "O campo \"Nome do Aluno\" deve possuir no máximo 50 caracteres."
            ));
        }

        return erros;
    }

    public override void Atualizar(Certificado entidadeAtualizada)
    {
        NomeAluno = entidadeAtualizada.NomeAluno.Trim();
        CaminhoArquivo = entidadeAtualizada.CaminhoArquivo;
        DataGeracao = entidadeAtualizada.DataGeracao;
        Status = entidadeAtualizada.Status;
    }

    public void RegistrarGeracao(string caminhoArquivo)
    {
        if (string.IsNullOrWhiteSpace(caminhoArquivo))
        {
            throw new ArgumentException(
                "O caminho do arquivo gerado é obrigatório.",
                nameof(caminhoArquivo)
            );
        }

        CaminhoArquivo = caminhoArquivo;
        Status = StatusCertificado.Gerado;
        DataGeracao = DateTime.UtcNow;
    }

    public void RegistrarFalha()
    {
        Status = StatusCertificado.Falha;
    }
}