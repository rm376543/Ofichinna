namespace Ofichina.Contracts.Responses.Authentication;

/// <summary>
/// Resposta do processo de autenticação.
/// </summary>
public class AuthenticationResponse : JwtResponse
{
    public Guid UsuarioId { get; set; }

    public string Email { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Perfis { get; set; } = [];

    public IReadOnlyCollection<string> Permissoes { get; set; } = [];
}