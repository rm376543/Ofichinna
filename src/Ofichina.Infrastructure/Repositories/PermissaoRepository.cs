using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces;
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
}
