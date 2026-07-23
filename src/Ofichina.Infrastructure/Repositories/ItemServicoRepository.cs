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
                .Include(x => x.Pecas)
                    .ThenInclude(x => x.Peca);
        }

        return await query.FirstOrDefaultAsync(
            x => x.OrdemServicoId == ordemServicoId && x.Id == id,
            cancellationToken);
    }

    public async Task<ItemServico?> GetByOrdemServicoIdAndServicoPecaIdAsync(
        Guid ordemServicoId,
        Guid pecaServicoId,
        CancellationToken cancellationToken = default,
        bool tracking = false)
    {
        // Método obsoleto - manter para compatibilidade mas retornar null pois não há mais ServicoPecaId único
        return null;
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
        Guid pecaServicoId,
        CancellationToken cancellationToken = default)
    {
        // Método obsoleto - criar item vazio e deixar handler adicionar peças
        var item = ItemServico.Criar(ordemServicoId);
        await AddAsync(item, cancellationToken);

        return item;
    }
}
