using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Interfaces;

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
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Busca uma entidade por seu Id.
    /// </summary>
    Task<TEntity?> GetByIdAsync(Guid id);

    /// <summary>
    /// Obtém todas as entidades.
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// Atualiza uma entidade.
    /// </summary>
    Task UpdateAsync(TEntity entity);

    /// <summary>
    /// Remove uma entidade.
    /// </summary>
    Task DeleteAsync(TEntity entity);
}
