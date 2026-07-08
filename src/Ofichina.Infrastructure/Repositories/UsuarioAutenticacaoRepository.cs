using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Shared;
using Ofichina.Infrastructure.Persistence;
using Ofichinna.Authentication.Abstractions;

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
        var normalizedEmail = Email.Criar(email).Value;

        return await _context.Usuarios
            .AsNoTracking()
            .Include(x => x.Perfis)
                .ThenInclude(x => x.Perfil)
            .FirstOrDefaultAsync(x => EF.Property<string>(x, nameof(Usuario.Email)) == normalizedEmail, cancellationToken);
    }
}