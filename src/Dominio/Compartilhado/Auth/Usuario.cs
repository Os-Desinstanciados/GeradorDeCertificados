namespace GeradorDeCertificados.Dominio.Compartilhado.Auth;

public sealed class Usuario
{
    public Guid UserId { get; set; }
    public string Nome { get; set; } = string.Empty;
}