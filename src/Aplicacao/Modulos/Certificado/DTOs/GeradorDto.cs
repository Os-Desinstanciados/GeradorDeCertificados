using GeradorDeCertificados.Dominio.Modulos.Certificado;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificado.DTOs;

public record GeradorDto(
    Guid Id,
    Guid CursoId,
    StatusGerador Status,
    string? CaminhoZip,
    IReadOnlyList<CertificadoDto> Certificados
);