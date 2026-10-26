using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;

public record StatusGeracaoDto(
    Guid GeradorId,
    Guid CursoId,
    StatusGerador Status,
    int TotalCertificados,
    int Gerados,
    int Falhas,
    bool ZipDisponivel,
    DateTime IniciadoEm,
    DateTime? ConcluidoEm
);