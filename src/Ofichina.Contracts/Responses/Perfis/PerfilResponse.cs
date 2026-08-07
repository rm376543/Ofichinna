using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Perfis;

/// <summary>
/// Resposta com os dados de um perfil.
/// </summary>
public class PerfilResponse : BaseEntity
{
    public Guid PerfilId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }
}
