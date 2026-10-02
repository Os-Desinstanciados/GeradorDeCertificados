namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public enum StatusGerador
{
    Pendente = 0,
    GerandoCertificados = 1,
    GerandoZip = 2,
    Concluido = 3,
    Falha = 4
}