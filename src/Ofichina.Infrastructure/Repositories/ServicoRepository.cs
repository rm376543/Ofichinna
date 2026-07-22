using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para consultas de serviços com suas peças.
/// </summary>
public sealed class ServicoRepository : Repository<Servico>, IServicoRepository
{
    private readonly ApplicationDbContext _context;

    public ServicoRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Servico?> GetByIdAsync(Guid id, bool includePecas = false, CancellationToken cancellationToken = default, bool tracking = false)
    {
        IQueryable<Servico> query = _context.Set<Servico>();

        if (!tracking)
            query = query.AsNoTracking();

        if (includePecas)
            query = query.Include(x => x.Pecas);

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Servico>> GetAllAsync(bool includePecas = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Servico> query = _context.Set<Servico>().AsNoTracking();

        if (includePecas)
            query = query.Include(x => x.Pecas);

        return await query.ToListAsync(cancellationToken);
    }
}