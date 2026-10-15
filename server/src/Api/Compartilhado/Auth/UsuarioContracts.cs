namespace GeradorDeCertificados.WebApi.Compartilhado.Auth;

public sealed record CadastrarUsuarioRequest(
    string Email,
    string Senha
);

public sealed record CadastrarUsuarioResponse(
    Guid Id
);

public sealed record AutenticarUsuarioRequest(
    string Email,
    string Senha
);

public sealed record AutenticarUsuarioResponse(
    Guid UsuarioId,
    string AccessToken,
    DateTime DataExpiracaoEmUtc
);