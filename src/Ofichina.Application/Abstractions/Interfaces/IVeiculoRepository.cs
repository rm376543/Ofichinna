using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Interface específica para o repositório de Veiculo.
/// </summary>
public interface IVeiculoRepository : IRepository<Veiculo>
{
    /// <summary>
    /// Obtém um veículo pelo identificador, incluindo a pessoa vinculada.
    /// </summary>
    Task<Veiculo?> GetByIdWithPessoaAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todos os veículos de uma pessoa pelo identificador da pessoa, incluindo a pessoa vinculada.
    /// </summary>
    Task<PagedResult<Veiculo>> GetVeiclesPagedByPessoaIdAsync(
        Guid pessoaId,
        Pagination pagination,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todos os veículos, incluindo a pessoa vinculada.
    /// </summary>
    Task<IEnumerable<Veiculo>> GetAllWithPessoaAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista veículos paginados, incluindo a pessoa vinculada.
    /// </summary>
    Task<PagedResult<Veiculo>> GetPagedWithPessoaAsync(Pagination pagination, CancellationToken cancellationToken = default);
}