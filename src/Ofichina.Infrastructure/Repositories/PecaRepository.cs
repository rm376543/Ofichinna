using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories
{
    public sealed class PecaRepository : Repository<Peca>, IPecaRepository
    {
        private readonly ApplicationDbContext _context;

        public PecaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca uma lista paginada de entidades Peca com base nos parâmetros de paginação fornecidos.
        /// </summary>
        /// <param name="pagination">Os parâmetros de paginação.</param>
        /// <param name="cancellationToken">O token de cancelamento.</param>
        /// <returns>Uma resposta paginada contendo entidades Peca.</returns>
    public new async Task<PagedResponse<Peca>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(pagination);

            var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
            var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

            var query = _context.Pecas
                .AsNoTracking()
                .Where(x => x.DeletedAt == null)
                .OrderBy(x => x.Nome);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return items.ToPagedResponse(totalCount, pageNumber, pageSize);
        }
    }
}
