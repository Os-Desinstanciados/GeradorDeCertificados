using GeradorDeCertificados.Dominio.Modulos.Certificado;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificado.DTOs;

public record CertificadoDto(
    Guid Id,
    string NomeAluno,
    string? CaminhoArquivo,
    DateTime? DataGeracao,
    StatusCertificado Status
);