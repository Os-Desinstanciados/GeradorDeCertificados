using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public sealed class Gerador : EntidadeBase<Gerador>
{
    private readonly List<Certificado> certificados = [];

    public Guid CursoId { get; private set; }
    public StatusGerador Status { get; private set; }
    public string? CaminhoZip { get; private set; }
    public DateTime DataSolicitacao { get; private set; }
    public IReadOnlyCollection<Certificado> Certificados => certificados;

    public bool EstaEmAndamento =>
        Status is StatusGerador.Pendente
            or StatusGerador.GerandoCertificados
            or StatusGerador.GerandoZip;

     public bool EstaFinalizado =>
        Status is StatusGerador.Concluido or StatusGerador.Falha;

    private Gerador() { }

    public Gerador(Guid id, Guid cursoId, IEnumerable<string> nomesAlunos)
    {
        Id = id;
        CursoId = cursoId;
        Status = StatusGerador.Pendente;
        DataSolicitacao = DateTime.UtcNow;

        foreach (string nome in nomesAlunos ?? [])
        {
            certificados.Add(new Certificado(Guid.CreateVersion7(), id, nome));
        }
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (certificados.Count == 0)
        {
            erros.Add(new ErroValidacao(
                "Alunos",
                "A solicitação deve possuir pelo menos um aluno."
            ));
        }

        foreach (Certificado certificado in certificados)
        {
            erros.AddRange(certificado.Validar());
        }

        return erros;
    }

    public override void Atualizar(Gerador entidadeAtualizada)
    {
        Status = entidadeAtualizada.Status;
        CaminhoZip = entidadeAtualizada.CaminhoZip;
    }

    public void GerarCertificados()
    {
        if (Status != StatusGerador.Pendente)
        {
            throw new InvalidOperationException(
                "O gerador está ocupado ou ouve uma falha."
            );
        }

        Status = StatusGerador.GerandoCertificados;
    }

    public void MarcarComoGerandoCertificados()
    {
        Status = StatusGerador.GerandoCertificados;
    }

    public void MarcarComoGerandoZip()
    {
        Status = StatusGerador.GerandoZip;
    }

    public void MarcarComoConcluido(string caminhoZip)
    {
        CaminhoZip = caminhoZip;
        Status = StatusGerador.Concluido;
    }

    public void MarcarComoFalha()
    {
        Status = StatusGerador.Falha;
    }
}