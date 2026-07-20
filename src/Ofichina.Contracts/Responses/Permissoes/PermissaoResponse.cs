namespace Ofichina.Contracts.Responses.Permissoes;

public sealed class PermissaoResponse
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}
