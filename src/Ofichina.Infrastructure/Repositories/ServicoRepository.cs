using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para consultas de serviços.
/// </summary>
public sealed class ServicoRepository : Repository<Servico>, IServicoRepository
{
    private readonly ApplicationDbContext _context;

    public ServicoRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtém um serviço pelo seu ID.
    /// </summary>
    /// <param name="id">O identificador do serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <param name="tracking">Indica se o rastreamento de alterações deve ser habilitado.</param>
    /// <returns>O serviço correspondente ao ID fornecido, ou nulo se não encontrado.</returns>
    public async Task<Servico?> GetByIdAsync(Guid id, bool includePecas = false, CancellationToken cancellationToken = default, bool tracking = false)
    {
        IQueryable<Servico> query = _context.Set<Servico>();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <summary>
    /// Obtém todos os serviços.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Uma lista de serviços.</returns>
    public async Task<IReadOnlyCollection<Servico>> GetAllAsync(bool includePecas = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Servico> query = _context.Set<Servico>().AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtém todos os serviços de forma paginada.
    /// </summary>
    /// <param name="pagination">Parâmetros de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Uma lista paginada de serviços.</returns>
    public async Task<PagedResponse<Servico>> GetAllServicosPaginadosAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.Set<Servico>()
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