namespace Ofichina.Contracts.Requests.Perfil;

/// <summary>
/// Requisição para atualização de perfil.
/// </summary>
public class UpdatePerfilRequest : UpdateRequest
{
    public string Codigo { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public bool Ativo { get; set; }
}