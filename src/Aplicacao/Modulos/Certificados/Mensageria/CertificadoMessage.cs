namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;

public sealed record CertificadoMessage(
    Guid CursoId,
    Guid GeradorId
);