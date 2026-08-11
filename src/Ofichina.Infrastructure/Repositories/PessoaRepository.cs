using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
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

        /// <summary>
        /// Busca uma pessoa pelo ID do usuário associado.
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A pessoa associada ao ID do usuário, ou null se não encontrada.</returns>
        public Task<Pessoa?> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return _context.Pessoas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId, cancellationToken);
        }

        /// <summary>
        /// Busca uma pessoa pelo seu ID, com a opção de incluir os veículos associados.
        /// </summary>
        /// <param name="pessoaId"></param>
        /// <param name="includeVeiculos"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A pessoa associada ao ID fornecido, ou null se não encontrada.</returns>
        public async Task<Pessoa?> GetByIdAsync(Guid pessoaId, bool includeVeiculos = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Pessoa> query = _context.Pessoas;

            if (!includeVeiculos)
                query = query.AsNoTracking();
            else
                query = query.AsNoTracking().Include(x => x.Veiculos);

            return await query.FirstOrDefaultAsync(x => x.Id == pessoaId, cancellationToken);
        }

        /// <summary>
        /// Busca uma coleção de pessoas pelos seus IDs.
        /// </summary>
        /// <param name="pessoaIds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Uma coleção de pessoas associadas aos IDs fornecidos.</returns>
        public async Task<IReadOnlyCollection<Pessoa>> GetByIdsAsync(IEnumerable<Guid> pessoaIds, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(pessoaIds);

            var ids = pessoaIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray();

            if (ids.Length == 0)
                return [];

            return await _context.Pessoas
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Busca uma página de pessoas com base na paginação fornecida.
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Uma página de pessoas com base na paginação fornecida.</returns>
        public new async Task<PagedResponse<Pessoa>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
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
