using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Interface específica para o repositório de Veiculo.
/// </summary>
public interface IVeiculoRepository : IRepository<Veiculo>
{
    /// <summary>
    /// Obtém um veículo pelo identificador, incluindo a pessoa vinculada quando solicitado.
    /// </summary>
    Task<Veiculo?> GetByIdAsync(Guid id, bool includePessoa = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todos os veículos de uma pessoa pelo identificador da pessoa, incluindo a pessoa vinculada.
    /// </summary>
    Task<IReadOnlyCollection<Veiculo>> GetAllVeiculosByPessoaIdAsync(
        Guid pessoaId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todos os veículos, incluindo a pessoa vinculada quando solicitado.
    /// </summary>
    Task<IEnumerable<Veiculo>> GetAllAsync(bool includePessoa = false, CancellationToken cancellationToken = default);
}