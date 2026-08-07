using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.Permissoes;

public sealed class CreatePermissaoRequest : CreateRequest
{
    public string Codigo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}
