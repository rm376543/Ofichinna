using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório específico para Veiculo.
/// </summary>
public class VeiculoRepository : Repository<Veiculo>, IVeiculoRepository
{
    private readonly ApplicationDbContext _context;

    public VeiculoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Veiculo?> GetByIdWithPessoaAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Veiculo>()
            .AsNoTracking()
            .Include(x => x.Pessoa)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Veiculo>> GetAllWithPessoaAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Veiculo>()
            .AsNoTracking()
            .Include(x => x.Pessoa)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Veiculo>> GetPagedWithPessoaAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.Set<Veiculo>()
            .AsNoTracking()
            .Include(x => x.Pessoa)
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Veiculo>(items, totalCount, pageNumber, pageSize);
    }
}