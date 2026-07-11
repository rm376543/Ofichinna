namespace Ofichina.Contracts.Responses;

/// <summary>
/// Dados básicos do JWT emitido.
/// </summary>
public class TokenJwtResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiraEm { get; set; }
}

/// <summary>
/// Resposta do processo de autenticação.
/// </summary>
public class AutenticacaoResponse : TokenJwtResponse
{
    public Guid UsuarioId { get; set; }

    public string Email { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Perfis { get; set; } = [];

}