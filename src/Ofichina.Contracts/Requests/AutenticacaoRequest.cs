namespace Ofichina.Contracts.Requests;

/// <summary>
/// Requisição de autenticação por e-mail e senha.
/// </summary>
public class AutenticacaoRequest
{
    public string Email { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;
}