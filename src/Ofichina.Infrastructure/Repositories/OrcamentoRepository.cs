using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para consultas do orçamento com itens e agendamento.
/// </summary>
public sealed class OrcamentoRepository : Repository<Orcamento>, IOrcamentoRepository
{
    private readonly ApplicationDbContext _context;

    public OrcamentoRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Orcamento?> GetByIdAsync(Guid id, bool includeItens = false, CancellationToken cancellationToken = default, bool tracking = false)
    {
        IQueryable<Orcamento> query = _context.Set<Orcamento>();

        if (!tracking)
            query = query.AsNoTracking();

        if (includeItens)
        {
            query = query
                .Include(x => x.Agendamento)
                .Include(x => x.ItensServico)
                    .ThenInclude(x => x.Servico)
                .Include(x => x.ItensServico)
                    .ThenInclude(x => x.Peca);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Orcamento>> GetAllAsync(bool includeItens = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Orcamento> query = _context.Set<Orcamento>().AsNoTracking();

        if (includeItens)
        {
            query = query
                .Include(x => x.Agendamento)
                .Include(x => x.ItensServico)
                    .ThenInclude(x => x.Servico)
                .Include(x => x.ItensServico)
                    .ThenInclude(x => x.Peca);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public new async Task<PagedResponse<Orcamento>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.Set<Orcamento>()
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
