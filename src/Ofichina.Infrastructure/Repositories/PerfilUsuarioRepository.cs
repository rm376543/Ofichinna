using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

public sealed class PerfilUsuarioRepository : Repository<UsuarioPerfil>, IPerfilUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public PerfilUsuarioRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> ExisteAsync(Guid clienteId, Guid perfilId)
    {
        return await _context.UsuariosPerfis
            .AsNoTracking()
            .AnyAsync(x => x.UsuarioId == clienteId && x.PerfilId == perfilId);
    }

    public async Task<UsuarioPerfil?> GetByUsuarioIdPerfilIdAsync(Guid usuarioId, Guid perfilId)
    {
        return await _context.UsuariosPerfis
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.PerfilId == perfilId);
    }
}