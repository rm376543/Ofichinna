namespace Ofichina.Contracts.Responses.PerfilPermissao;

public sealed class PerfilPermissaoResponse
{
    public Guid PerfilId { get; set; }

    public Guid PermissaoId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}
