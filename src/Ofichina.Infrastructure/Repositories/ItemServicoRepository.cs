using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
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

    public async Task<ItemServico?> GetByOrdemServicoIdAndIdAsync(
        Guid ordemServicoId,
        Guid id,
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
                .Include(x => x.PecaServico)
                    .ThenInclude(ps => ps.Peca)
                .Include(x => x.PecaServico)
                    .ThenInclude(ps => ps.Servico);
        }

        return await query.FirstOrDefaultAsync(
            x => x.OrdemServicoId == ordemServicoId && x.Id == id,
            cancellationToken);
    }

    public async Task<ItemServico?> GetByOrdemServicoIdAndPecaServicoIdAsync(
        Guid ordemServicoId,
        Guid pecaServicoId,
        CancellationToken cancellationToken = default,
        bool tracking = false)
    {
        IQueryable<ItemServico> query = _context.Set<ItemServico>();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(
            x => x.OrdemServicoId == ordemServicoId && x.PecaServicoId == pecaServicoId,
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
                .Include(x => x.PecaServico)
                    .ThenInclude(ps => ps.Peca)
                .Include(x => x.PecaServico)
                    .ThenInclude(ps => ps.Servico);
        }

        return await query
            .Where(x => x.OrdemServicoId == ordemServicoId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ItemServico> AdicionarAsync(
        Guid ordemServicoId,
        Guid pecaServicoId,
        CancellationToken cancellationToken = default)
    {
        var existente = await GetByOrdemServicoIdAndPecaServicoIdAsync(ordemServicoId, pecaServicoId, cancellationToken, tracking: true);

        if (existente is not null && !existente.EstaExcluida())
            throw new DomainException("A peça de serviço já foi adicionada à ordem de serviço.");

        var item = ItemServico.Criar(ordemServicoId, pecaServicoId);
        await AddAsync(item, cancellationToken);

        return item;
    }
}
