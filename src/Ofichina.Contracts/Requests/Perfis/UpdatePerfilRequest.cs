namespace Ofichina.Contracts.Requests.Perfis;

/// <summary>
/// Requisição para atualização de perfil.
/// </summary>
public class UpdatePerfilRequest : UpdateRequest
{
    public Guid PerfilId { get; set; }

    public string NomePerfil { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}
