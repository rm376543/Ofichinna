using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Permissoes;

public sealed class PermissaoResponse : BaseEntity
{
    public Guid PermissaoId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}
