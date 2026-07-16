using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Interfaces;

/// <summary>
/// Interface específica para o repositório de Veiculo.
/// </summary>
public interface IVeiculoRepository : IRepository<Veiculo>
{
    /// <summary>
    /// Obtém um veículo pelo identificador, incluindo a pessoa vinculada.
    /// </summary>
    Task<Veiculo?> GetByIdWithPessoaAsync(Guid id);

    /// <summary>
    /// Lista todos os veículos, incluindo a pessoa vinculada.
    /// </summary>
    Task<IEnumerable<Veiculo>> GetAllWithPessoaAsync();
}