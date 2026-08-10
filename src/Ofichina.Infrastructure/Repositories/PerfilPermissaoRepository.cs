using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

public sealed class PerfilPermissaoRepository : Repository<PerfilPermissao>, IPerfilPermissaoRepository
{
    private readonly ApplicationDbContext _context;

    public PerfilPermissaoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PerfilPermissao?> GetByPerfilIdPermissaoIdAsync(Guid perfilId, Guid permissaoId, CancellationToken cancellationToken = default)
    {
        return await _context.PerfisPermissoes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PerfilId == perfilId && x.PermissaoId == permissaoId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<PerfilPermissao>> GetByPerfilIdAsync(Guid perfilId, CancellationToken cancellationToken = default)
    {
        return await _context.PerfisPermissoes
            .AsNoTracking()
            .Include(x => x.Permissao)
            .Where(x => x.PerfilId == perfilId)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResponse<PerfilPermissao>> GetAllPermissoesAssociadosDeUmPerfil(
        Guid perfilId, Pagination pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.Set<PerfilPermissao>()
            .AsNoTracking()
            .Where(x => x.PerfilId == perfilId)
            .Include(x => x.Permissao)
            .OrderBy(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return items.ToPagedResponse(totalCount, pageNumber, pageSize);
    }
}
