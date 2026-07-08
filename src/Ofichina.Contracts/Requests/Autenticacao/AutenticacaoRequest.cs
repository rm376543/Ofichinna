namespace Ofichina.Contracts.Requests.Autenticacao;

/// <summary>
/// Requisição de autenticação por e-mail e senha.
/// </summary>
public class AutenticacaoRequest : BaseRequest
{
    public string Email { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;
}