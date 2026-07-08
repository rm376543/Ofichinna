using Microsoft.EntityFrameworkCore;
using Ofichina.Infrastructure.Persistence;
using Ofichinna.Authentication.Abstractions;

namespace Ofichina.Infrastructure.Services;

public sealed class PerfilAutorizacaoService : IPerfilAutorizacaoService
{
    private readonly ApplicationDbContext _context;

    public PerfilAutorizacaoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<string>> ObterPerfisAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _context.UsuariosPerfis
            .AsNoTracking()
            .Where(x => x.UsuarioId == usuarioId)
            .Select(x => x.Perfil.Codigo)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> PossuiPerfilAsync(Guid usuarioId, string perfil, CancellationToken cancellationToken = default)
    {
        var normalizedPerfil = perfil.Trim().ToUpperInvariant();

        return await _context.UsuariosPerfis
            .AsNoTracking()
            .AnyAsync(x => x.UsuarioId == usuarioId && x.Perfil.Codigo.ToUpper() == normalizedPerfil, cancellationToken);
    }
}