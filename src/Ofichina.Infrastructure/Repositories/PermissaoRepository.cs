using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

public sealed class PermissaoRepository : Repository<Permissao>, IPermissaoRepository
{
    private readonly ApplicationDbContext _context;

    public PermissaoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Permissao?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        var normalizedCodigo = codigo.Trim().ToUpperInvariant();

        return await _context.Set<Permissao>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Codigo.ToUpper() == normalizedCodigo, cancellationToken);
    }

    public async Task<PagedResponse<Permissao>> GetAllPermissoesPaginadasAsync(
        Pagination pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.Set<Permissao>()
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
