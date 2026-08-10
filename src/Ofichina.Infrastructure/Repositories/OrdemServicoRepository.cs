using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para consultas da ordem de serviço com os itens vinculados.
/// </summary>
public sealed class OrdemServicoRepository : Repository<OrdemServico>, IOrdemServicoRepository
{
    private readonly ApplicationDbContext _context;

    public OrdemServicoRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<OrdemServico?> GetByIdAsync(Guid id, bool includeItens = false, CancellationToken cancellationToken = default, bool tracking = false)
    {
        IQueryable<OrdemServico> query = _context.Set<OrdemServico>();

        if (!tracking)
            query = query.AsNoTracking();

        if (includeItens)
        {
            query = query
                .Include(x => x.Servicos)
                    .ThenInclude(x => x.Servico)
                .Include(x => x.Servicos)
                    .ThenInclude(x => x.Peca);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<OrdemServico>> GetAllAsync(bool includeItens = false, CancellationToken cancellationToken = default)
    {
        IQueryable<OrdemServico> query = _context.Set<OrdemServico>().AsNoTracking();

        if (includeItens)
        {
            query = query
                .Include(x => x.Servicos)
                    .ThenInclude(x => x.Servico)
                .Include(x => x.Servicos)
                    .ThenInclude(x => x.Peca);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public new async Task<PagedResponse<OrdemServico>> GetPagedAsync(
        Pagination pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.Set<OrdemServico>()
            .AsNoTracking()

            .OrderBy(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return items.ToPagedResponse(totalCount, pageNumber, pageSize);
    }
}
