using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Repositório responsável pela persistência das peças vinculadas aos serviços.
/// </summary>
public sealed class ServicoPecasRepository : Repository<ServicoPeca>, IServicoPecasRepository
{
    private readonly ApplicationDbContext _context;

    public ServicoPecasRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<ServicoPeca?> GetByServicoIdAndPecaIdAsync(
        Guid servicoId,
        Guid pecaId,
        CancellationToken cancellationToken = default,
        bool tracking = false)
    {
        IQueryable<ServicoPeca> query = _context.Set<ServicoPeca>();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(x => x.ServicoId == servicoId && x.PecaId == pecaId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ServicoPeca>> GetByServicoIdAsync(
        Guid servicoId,
        CancellationToken cancellationToken = default,
        bool includePeca = false,
        bool tracking = false)
    {
        IQueryable<ServicoPeca> query = _context.Set<ServicoPeca>();

        if (!tracking)
            query = query.AsNoTracking();

        if (includePeca)
            query = query.Include(x => x.Peca);

        return await query
            .Where(x => x.ServicoId == servicoId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ServicoPeca> AdicionarAsync(
        Guid servicoId,
        Guid pecaId,
        int quantidade,
        CancellationToken cancellationToken = default)
    {
        var existente = await GetByServicoIdAndPecaIdAsync(servicoId, pecaId, cancellationToken, tracking: true);

        if (existente is not null && !existente.EstaExcluida())
            throw new DomainException("A peça já foi adicionada ao serviço.");

        var servicoPeca = ServicoPeca.Criar(servicoId, pecaId, quantidade);
        await AddAsync(servicoPeca, cancellationToken);

        return servicoPeca;
    }
}