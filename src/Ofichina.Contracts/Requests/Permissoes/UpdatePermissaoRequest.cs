namespace Ofichina.Contracts.Requests.Permissoes;

public sealed class UpdatePermissaoRequest : UpdateRequest
{
    public string Codigo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}
