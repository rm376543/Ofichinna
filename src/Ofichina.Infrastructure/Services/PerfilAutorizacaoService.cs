using Microsoft.EntityFrameworkCore;
using Ofichina.Authentication.Abstractions;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Services;

public sealed class PerfilAutorizacaoService : IPerfilAutorizacaoService
{
    private readonly ApplicationDbContext _context;

    public PerfilAutorizacaoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<string>> ObterPerfisAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UsuariosPerfis
            .AsNoTracking()
            .Where(x => x.UsuarioId == usuarioId)
            .Select(x => x.Perfil.NomePerfil)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> PossuiPerfilAsync(
        Guid usuarioId,
        string perfil,
        CancellationToken cancellationToken = default)
    {
        var normalizedPerfil = perfil.Trim().ToUpperInvariant();

        return await _context.UsuariosPerfis
            .AsNoTracking()
            .AnyAsync(
                x => x.UsuarioId == usuarioId &&
                     x.Perfil.NomePerfil.ToUpper() == normalizedPerfil,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> ObterPermissoesAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UsuariosPerfis
            .AsNoTracking()
            .Where(x => x.UsuarioId == usuarioId)
            .SelectMany(x => x.Perfil.PerfisPermissoes)
            .Select(x => x.Permissao.Codigo)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> PossuiPermissaoAsync(
        Guid usuarioId,
        string permissao,
        CancellationToken cancellationToken = default)
    {
        var normalizedPermissao = permissao.Trim().ToUpperInvariant();

        return await _context.UsuariosPerfis
            .AsNoTracking()
            .Where(x => x.UsuarioId == usuarioId)
            .SelectMany(x => x.Perfil.PerfisPermissoes)
            .AnyAsync(
                x => x.Permissao.Codigo.ToUpper() == normalizedPermissao,
                cancellationToken);
    }
}