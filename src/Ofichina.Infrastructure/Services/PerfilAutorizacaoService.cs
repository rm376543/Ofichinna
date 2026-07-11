using Microsoft.EntityFrameworkCore;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Authentication.Abstractions;

namespace Ofichina.Infrastructure.Services;

public sealed class PerfilAutorizacaoService : IPerfilAutorizacaoService
{
    private readonly ApplicationDbContext _context;

    public PerfilAutorizacaoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<string>> ObterPerfisAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.UsuariosPerfis
            .AsNoTracking()
            .Where(x => x.UsuarioId == clienteId)
            .Select(x => x.Perfil.NomePerfil)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> PossuiPerfilAsync(Guid clienteId, string perfil, CancellationToken cancellationToken = default)
    {
        var normalizedPerfil = perfil.Trim().ToUpperInvariant();

        return await _context.UsuariosPerfis
            .AsNoTracking()
            .AnyAsync(x => x.UsuarioId == clienteId && x.Perfil.NomePerfil.ToUpper() == normalizedPerfil, cancellationToken);
    }
}