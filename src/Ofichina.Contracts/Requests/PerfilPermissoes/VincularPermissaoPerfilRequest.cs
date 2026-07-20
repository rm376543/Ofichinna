namespace Ofichina.Contracts.Requests.PerfilPermissoes;

public sealed class VincularPermissaoPerfilRequest : CreateRequest
{
    public Guid PerfilId { get; set; }

    public Guid PermissaoId { get; set; }
}
