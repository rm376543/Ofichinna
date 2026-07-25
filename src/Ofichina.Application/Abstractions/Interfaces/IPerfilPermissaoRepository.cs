using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IPerfilPermissaoRepository : IRepository<PerfilPermissao>
{
    Task<PerfilPermissao?> GetByPerfilIdPermissaoIdAsync(Guid perfilId, Guid permissaoId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PerfilPermissao>> GetByPerfilIdAsync(Guid perfilId, CancellationToken cancellationToken = default);
}
