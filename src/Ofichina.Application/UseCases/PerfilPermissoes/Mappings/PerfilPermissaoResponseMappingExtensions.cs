using Ofichina.Contracts.Extension;
using Ofichina.Contracts.Responses.PerfilPermissoes;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.PerfilPermissoes.Mappings;

public static class PerfilPermissaoResponseMappingExtensions
{
    public static PerfilPermissaoResponse ToResponse(this PerfilPermissao perfilPermissao)
    {
        ArgumentNullException.ThrowIfNull(perfilPermissao);

        return new PerfilPermissaoResponse
        {
            PerfilPermissaoId = perfilPermissao.Id,
            PerfilId = perfilPermissao.PerfilId,
            PermissaoId = perfilPermissao.PermissaoId,
            Codigo = perfilPermissao.Permissao?.Codigo ?? string.Empty,
            Descricao = perfilPermissao.Permissao?.Descricao ?? string.Empty,
            CreatedAt = perfilPermissao.CreatedAt.ToDateString(),
            UpdatedAt = perfilPermissao.UpdatedAt?.ToDateString(),
            DeletedAt = perfilPermissao.DeletedAt?.ToDateString()
        };
    }
}
