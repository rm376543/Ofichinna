using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Application.Abstractions.Authentication;

namespace Ofichina.Infrastructure.Repositories;

public sealed class UsuarioAutenticacaoRepository : IUsuarioAutenticacaoRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioAutenticacaoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        Email normalizedEmail = new Email(email);

        return await _context.Usuarios
            .AsNoTracking()
            .Include(x => x.Perfis)
                .ThenInclude(x => x.Perfil)
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
    }
}