using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para consultas da ordem de serviço com os itens vinculados.
/// </summary>
public sealed class OrdemServicoRepository : IOrdemServicoRepository
{
    private readonly ApplicationDbContext _context;

    public OrdemServicoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrdemServico?> GetByIdAsync(Guid id, bool includeItens = false)
    {
        IQueryable<OrdemServico> query = _context.Set<OrdemServico>();

        if (includeItens)
        {
            query = query
                .Include(x => x.Servicos)
                .Include(x => x.Pecas);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyCollection<OrdemServico>> GetAllAsync(bool includeItens = false)
    {
        IQueryable<OrdemServico> query = _context.Set<OrdemServico>();

        if (includeItens)
        {
            query = query
                .Include(x => x.Servicos)
                .Include(x => x.Pecas);
        }

        return await query.ToListAsync();
    }
}
