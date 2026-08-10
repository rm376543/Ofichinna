using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces.Repository;

public interface IPerfilPermissaoRepository : IRepository<PerfilPermissao>
{
    /// <summary>
    /// Buscar uma permissão associada a um perfil específico pelo ID do perfil e ID da permissão.
    /// </summary>
    /// <param name="perfilId"></param>
    /// <param name="permissaoId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Retorna a permissão associada ao perfil, se existir.</returns>
    Task<PerfilPermissao?> GetByPerfilIdPermissaoIdAsync(Guid perfilId, Guid permissaoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Buscar todas as permissões associadas a um perfil específico pelo ID do perfil.
    /// </summary>
    /// <param name="perfilId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Retorna todas as permissões associadas ao perfil.</returns>
    Task<IReadOnlyCollection<PerfilPermissao>> GetByPerfilIdAsync(Guid perfilId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Buscar todos os perfis associados a um perfil específico pelo ID do perfil.
    /// </summary>
    /// <param name="perfilId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Retorna todos os perfis associados ao perfil.</returns>
    Task<PagedResponse<PerfilPermissao>> GetAllPermissoesAssociadosDeUmPerfil(Guid perfilId, Pagination pagination, CancellationToken cancellationToken = default);
}
