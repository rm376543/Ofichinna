using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Implementação genérica do repositório.
/// Fornece operações básicas de CRUD para todas as entidades.
/// </summary>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task AddAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _dbSet.AddAsync(entity);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }
}
