using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Aggregates;
using Ofichina.Application.Abstractions.Interfaces;
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
                    .ThenInclude(s => s.Pecas);
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
                    .ThenInclude(s => s.Pecas);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
