using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories
{
    public class PessoaRepository : Repository<Pessoa>, IPessoaRepository
    {
        private readonly ApplicationDbContext _context;
        public PessoaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
