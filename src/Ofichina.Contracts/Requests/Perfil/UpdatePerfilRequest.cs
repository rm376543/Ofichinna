namespace Ofichina.Contracts.Requests.Perfil;

/// <summary>
/// Requisição para atualização de perfil.
/// </summary>
public class UpdatePerfilRequest : UpdateRequest
{
    public string NomePerfil { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}