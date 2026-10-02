using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public sealed class Gerador : EntidadeBase<Gerador>
{
    private readonly List<Certificado> certificados = [];

    public Guid CursoId { get; private set; }
    public StatusGerador Status { get; private set; }
    public string? CaminhoZip { get; private set; }
    public DateTime DataSolicitacao { get; private set; }
    public DateTime? ConcluidoEm { get; private set; }
    public IReadOnlyCollection<Certificado> Certificados => certificados;

    public int Gerados => certificados.Count(c => c.Status == StatusCertificado.Gerado);
    public int Falhas => certificados.Count(c => c.Status == StatusCertificado.Falha);
    public bool TodosCertificadosProcessados =>
        certificados.Count > 0 && certificados.All(c => c.Status != StatusCertificado.Pendente);
    public bool EstaFinalizado =>
        Status is StatusGerador.Concluido or StatusGerador.Falha;

    public bool EstaEmAndamento =>
        Status is StatusGerador.Pendente
            or StatusGerador.GerandoCertificados
            or StatusGerador.GerandoZip;    

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

    public void RegistrarSucesso(Guid certificadoId, string caminhoArquivo)
    {
        Certificado certificado = EncontrarPendente(certificadoId);
        certificado.RegistrarGeracao(caminhoArquivo);
    }

    public void RegistrarFalha(Guid certificadoId)
    {
        Certificado certificado = EncontrarPendente(certificadoId);
        certificado.RegistrarFalha();
    }

    public void IniciarGeracaoCertificados()
    {
        if (Status != StatusGerador.Pendente)
        {
            throw new InvalidOperationException(
                "Só é possível iniciar a geração de certificados a partir do status Pendente."
            );
        }

        Status = StatusGerador.GerandoCertificados;
    }

    public void IniciarGeracaoZip()
    {
        if (Status != StatusGerador.GerandoCertificados)
        {
            throw new InvalidOperationException(
                "Só é possível iniciar a geração do ZIP a partir do status GerandoCertificados."
            );
        }

        if (!TodosCertificadosProcessados)
        {
            throw new InvalidOperationException(
                "Não é possível iniciar a geração do ZIP enquanto houver certificados pendentes."
            );
        }

        if (Gerados == 0)
        {
            throw new InvalidOperationException(
                "Não é possível iniciar a geração do ZIP sem pelo menos um certificado gerado."
            );
        }

        Status = StatusGerador.GerandoZip;
    }

    public void RegistrarZip(string caminhoZip)
    {
        if (Status != StatusGerador.GerandoZip)
        {
            throw new InvalidOperationException(
                "Só é possível registrar o ZIP a partir do status GerandoZip."
            );
        }

        if (!TodosCertificadosProcessados)
        {
            throw new InvalidOperationException(
                "Não é possível registrar o ZIP enquanto houver certificados pendentes."
            );
        }

        if (string.IsNullOrWhiteSpace(caminhoZip))
        {
            throw new ArgumentException(
                "O caminho do arquivo ZIP é obrigatório.",
                nameof(caminhoZip)
            );
        }

        CaminhoZip = caminhoZip;
        Status = StatusGerador.Concluido;
        ConcluidoEm = DateTime.UtcNow;
    }

    public void RegistrarFalhaGerador()
    {
        if (EstaFinalizado)
        {
            throw new InvalidOperationException(
                "Não é possível registrar falha em um gerador já finalizado."
            );
        }

        Status = StatusGerador.Falha;
        ConcluidoEm = DateTime.UtcNow;
    }

    private Certificado EncontrarPendente(Guid certificadoId)
    {
        return certificados.Single(c =>
            c.Id == certificadoId && c.Status == StatusCertificado.Pendente
        );
    }
}