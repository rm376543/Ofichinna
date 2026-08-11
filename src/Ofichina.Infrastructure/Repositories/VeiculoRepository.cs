using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Contracts.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório específico para Veiculo.
/// </summary>
public class VeiculoRepository : Repository<Veiculo>, IVeiculoRepository
{
    private readonly ApplicationDbContext _context;

    public VeiculoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtém um veículo pelo seu ID, incluindo a pessoa associada.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Veiculo?> GetByIdAsync(Guid id, bool includePessoa = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Veiculo> query = _context.Set<Veiculo>();

        if (!includePessoa)
            query = query.AsNoTracking();
        else
            query = query.AsNoTracking().Include(x => x.Pessoa);

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <summary>
    /// Obtém todos os veículos com suas respectivas pessoas associadas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de veículos com suas respectivas pessoas associadas.</returns>
    public async Task<IEnumerable<Veiculo>> GetAllAsync(bool includePessoa = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Veiculo> query = _context.Set<Veiculo>().AsNoTracking();

        if (includePessoa)
            query = query.Include(x => x.Pessoa);

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtém todos os veículos associados a uma pessoa específica pelo ID da pessoa.
    /// </summary>
    /// <param name="pessoaId">ID da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de veículos associados à pessoa.</returns>
    public async Task<IReadOnlyCollection<Veiculo>> GetAllVeiculosByPessoaIdAsync(
        Guid pessoaId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Veiculo>()
            .AsNoTracking()
            .Include(x => x.Pessoa)
            .Where(x => x.PessoaId == pessoaId && x.DeletedAt == null)
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtém uma lista paginada de veículos ativos com base nos parâmetros de paginação fornecidos.
    /// </summary>
    /// <param name="pagination">Parâmetros de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado paginado de veículos.</returns>
    public new async Task<PagedResponse<Veiculo>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.Set<Veiculo>()
            .AsNoTracking()
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return items.ToPagedResponse(totalCount, pageNumber, pageSize);
    }
}