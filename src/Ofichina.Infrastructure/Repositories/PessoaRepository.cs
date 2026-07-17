using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
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

        public Task<Pessoa?> GetByUsuarioIdAsync(Guid usuarioId)
        {
            return _context.Pessoas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && !x.EstaExcluida());
        }
    }
}
