using Ofichina.Domain.Entities;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;

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

        public Task<Pessoa?> GetByIdWithVeiculosAsync(Guid pessoaId, CancellationToken cancellationToken = default)
        {
            return _context.Pessoas
                .AsNoTracking()
                .Include(x => x.Veiculos)
                .FirstOrDefaultAsync(x => x.Id == pessoaId, cancellationToken);
        }

        public async Task<PagedResponse<Pessoa>> GetAllPessoasPaginadasAsync(Pagination pagination, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(pagination);

            var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
            var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

            var query = _context.Set<Pessoa>()
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return items.ToPagedResponse(totalCount, pageNumber, pageSize);
        }
    }
}
