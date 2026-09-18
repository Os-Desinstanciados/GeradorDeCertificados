using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Curso;

public sealed class Curso : EntidadeBase<Curso>
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public int CargaHoraria { get; private set; }
    public DateTime DataConclusao { get; private set; }

    private Curso() { }

    public Curso(
        Guid id,
        string nome,
        string? descricao,
        int cargaHoraria,
        DateTime dataConclusao)
    {
        Id = id;
        Nome = nome.Trim();
        Descricao = descricao;
        CargaHoraria = cargaHoraria;
        DataConclusao = dataConclusao;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(Nome))
        {
            erros.Add(new ErroValidacao(
                nameof(Nome),
                "O nome deve ser preenchido."
            ));
        }

        else if (Nome.Length is < 2 or > 100)
        {
            erros.Add(new ErroValidacao(
                nameof(Nome),
                "O nome deve possuir entre 2 e 100 caracteres."
            ));
        }

        if (Descricao?.Length > 500)
        {
            erros.Add(new ErroValidacao(
                nameof(Descricao),
                "A descrição deve possuir no máximo 500 caracteres."
            ));
        }

        if (CargaHoraria <= 0)
        {
            erros.Add(new ErroValidacao(
                nameof(CargaHoraria),
                "A carga horária deve ser maior que zero."
            ));
        }

        if (DataConclusao == default)
        {
            erros.Add(new ErroValidacao(
                nameof(DataConclusao),
                "A data de conclusão deve ser preenchida."
            ));
        }

        return erros;
    }

    public override void Atualizar(Curso entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome.Trim();
        Descricao = entidadeAtualizada.Descricao;
        CargaHoraria = entidadeAtualizada.CargaHoraria;
        DataConclusao = entidadeAtualizada.DataConclusao;
    }
}