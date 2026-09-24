namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;

public record ArquivoCertificadoDto(
    Stream Stream,
    string Name,
    string ContentType
);