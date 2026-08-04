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
                .Include(x => x.Servico)
                .Include(x => x.Peca);
        }

        return await query.FirstOrDefaultAsync(
            x => x.OrdemServicoId == ordemServicoId && x.Id == itemServicoId,
            cancellationToken);
    }

    public async Task<ItemServico?> GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(
        Guid ordemServicoId,
        Guid servicoId,
        Guid pecaId,
        CancellationToken cancellationToken = default,
        bool tracking = false)
    {
        IQueryable<ItemServico> query = _context.Set<ItemServico>();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(
            x => x.OrdemServicoId == ordemServicoId && x.ServicoId == servicoId && x.PecaId == pecaId,
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
                .Include(x => x.Servico)
                .Include(x => x.Peca);
        }

        return await query
            .Where(x => x.OrdemServicoId == ordemServicoId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ItemServico> AddAsync(
        Guid ordemServicoId,
        Guid servicoId,
        Guid pecaId,
        int quantidade,
        CancellationToken cancellationToken = default)
    {
        var item = ItemServico.ParaOrdemServico(ordemServicoId, servicoId, pecaId, quantidade);
        await AddAsync(item, cancellationToken);

        return item;
    }

    public async Task<IReadOnlyCollection<ItemServico>> GetByOrdemServicoIdAndServicoIdAsync(
        Guid ordemServicoId,
        Guid servicoId,
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
                .Include(x => x.Servico)
                .Include(x => x.Peca);
        }

        return await query
            .Where(x =>
                x.OrdemServicoId == ordemServicoId &&
                x.ServicoId == servicoId &&
                x.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }
}
