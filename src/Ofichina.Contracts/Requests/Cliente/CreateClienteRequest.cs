namespace Ofichina.Contracts.Requests.Usuario;

/// <summary>
/// Requisição para cadastro de novo usuário.
/// </summary>
public class CreateClienteRequest : CreateRequest
{
    public string Email { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;
}