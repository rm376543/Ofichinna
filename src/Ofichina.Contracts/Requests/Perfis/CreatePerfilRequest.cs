using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.Perfis;

/// <summary>
/// Requisição para criação de perfil.
/// </summary>
public class CreatePerfilRequest : CreateRequest
{
    public string NomePerfil { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}
