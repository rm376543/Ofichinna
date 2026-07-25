using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Repositório responsável pela persistência dos itens de serviço.
/// </summary>
public sealed class ItemServicoRepository : Repository<ItemServico>, IItemServicoRepository
{
    private readonly ApplicationDbContext _context;

    public ItemServicoRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<ItemServico?> GetByOrdemServicoIdAndItemServicoIdAsync(
        Guid ordemServicoId,
        Guid itemServicoId,
        CancellationToken cancellationToken = default,
        bool tracking = false,
        bool includeRelacionados = false)
    {
        IQueryable<ItemServico> query = _context.Set<ItemServico>();

        if (!tracking)
            query = query.AsNoTracking();

        if (includeRelacionados)
        {
            query = query
                .Include(x => x.Pecas)
                    .ThenInclude(x => x.Peca);
        }

        return await query.FirstOrDefaultAsync(
            x => x.OrdemServicoId == ordemServicoId && x.Id == itemServicoId,
            cancellationToken);
    }

    public async Task<ItemServico?> GetByOrdemServicoIdAndServicoPecaIdAsync(
        Guid ordemServicoId,
        Guid servicoPecaId,
        CancellationToken cancellationToken = default,
        bool tracking = false)
    {
        IQueryable<ItemServico> query = _context.Set<ItemServico>();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(
            x => x.OrdemServicoId == ordemServicoId && x.ServicoPecaId == servicoPecaId,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<ItemServico>> GetByOrdemServicoIdAsync(
        Guid ordemServicoId,
        CancellationToken cancellationToken = default,
        bool includeRelacionados = false,
        bool tracking = false)
    {
        IQueryable<ItemServico> query = _context.Set<ItemServico>();

        if (!tracking)
            query = query.AsNoTracking();

        if (includeRelacionados)
        {
            query = query
                .Include(x => x.Pecas)
                    .ThenInclude(x => x.Peca);
        }

        return await query
            .Where(x => x.OrdemServicoId == ordemServicoId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ItemServico> AdicionarAsync(
        Guid ordemServicoId,
        Guid servicoPecaId,
        CancellationToken cancellationToken = default)
    {
        var item = ItemServico.Criar(ordemServicoId);
        await AddAsync(item, cancellationToken);

        return item;
    }
}
