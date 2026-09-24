using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;

public record GeradorDto(
    Guid Id,
    Guid CursoId,
    StatusGerador Status,
    string? CaminhoZip,
    IReadOnlyList<CertificadoDto> Certificados
);