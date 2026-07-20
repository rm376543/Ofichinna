using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces;
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
}
