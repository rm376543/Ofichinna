using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório específico para Perfil.
/// </summary>
public class PerfilRepository : Repository<Perfil>, IPerfilRepository
{
    private readonly ApplicationDbContext _context;

    public PerfilRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Perfil?> GetByNomeAsync(string nomePerfil, CancellationToken cancellationToken = default)
    {
        var normalizedNome = nomePerfil.Trim().ToUpperInvariant();

        return await _context.Set<Perfil>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NomePerfil.ToUpper() == normalizedNome, cancellationToken);
    }

    public async Task<IEnumerable<Perfil>> GetAllAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllAsync(cancellationToken);
    }
}