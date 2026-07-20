using Ofichina.Domain.Entities;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ofichina.Infrastructure.Repositories
{
    public class PessoaRepository : Repository<Pessoa>, IPessoaRepository
    {
        private readonly ApplicationDbContext _context;
        public PessoaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<Pessoa?> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return _context.Pessoas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId, cancellationToken);
        }
    }
}
