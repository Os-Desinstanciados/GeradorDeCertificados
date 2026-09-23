using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Api.Modulos.Certificados;

public sealed record GerarCertificadosRequest(
    IReadOnlyList<AlunoRequest> Alunos
);

public sealed record AlunoRequest(string Nome);

public sealed record GerarCertificadosResponse(
    Guid GeradorId,
    StatusGerador Status
);

public sealed record StatusGeradorResponse(
    Guid GeradorId,
    Guid CursoId,
    StatusGerador Status
);

public sealed record CertificadoResponse(
    Guid Id,
    string NomeAluno,
    DateTime? DataGeracao,
    StatusCertificado Status
);