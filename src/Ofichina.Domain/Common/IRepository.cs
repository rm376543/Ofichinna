using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Common;

/// <summary>
/// Interface genérica para repositórios.
/// Define as operações básicas de persistência de dados.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade.</typeparam>
public interface IRepository<TEntity> where TEntity : Entity
{
    /// <summary>
    /// Adiciona uma entidade ao repositório.
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca uma entidade por seu Id.
    /// </summary>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracking = false);

    /// <summary>
    /// Obtém todas as entidades.
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém uma página de entidades.
    /// </summary>
    Task<PagedResult<TEntity>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma entidade.
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove uma entidade.
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove fisicamente uma entidade.
    /// </summary>
    Task HardDeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
