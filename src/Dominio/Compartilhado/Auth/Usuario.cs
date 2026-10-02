namespace GeradorDeCertificados.Dominio.Compartilhado.Auth;

public sealed class Usuario
{
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
}