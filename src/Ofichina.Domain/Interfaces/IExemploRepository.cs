using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Interfaces;

/// <summary>
/// Interface específica para o repositório de Exemplo.
/// Estende o repositório genérico com métodos específicos do domínio.
/// </summary>
public interface IExemploRepository : IRepository<Exemplo>
{
    /// <summary>
    /// Busca um exemplo por nome.
    /// </summary>
    Task<Exemplo?> GetByNameAsync(string nome);

    /// <summary>
    /// Busca todos os exemplos ativos.
    /// </summary>
    Task<IEnumerable<Exemplo>> GetAllAtivosAsync();
}
