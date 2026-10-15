using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;

public record StatusGeracaoDto(
    Guid GeradorId,
    Guid CursoId,
    StatusGerador Status
);