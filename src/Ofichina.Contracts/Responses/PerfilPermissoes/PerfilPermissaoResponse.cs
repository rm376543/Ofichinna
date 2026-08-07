using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.PerfilPermissoes;

public sealed class PerfilPermissaoResponse : BaseResponse
{
    public Guid PerfilPermissaoId { get; set; }

    public Guid PerfilId { get; set; }

    public Guid PermissaoId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}
