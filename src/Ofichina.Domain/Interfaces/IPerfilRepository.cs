using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Interfaces;

/// <summary>
/// Interface específica para o repositório de Perfil.
/// </summary>
public interface IPerfilRepository : IRepository<Perfil>
{
    /// <summary>
    /// Busca um perfil pelo código.
    /// </summary>
    Task<Perfil?> GetByCodigoAsync(string codigo);

    /// <summary>
    /// Lista todos os perfis ativos.
    /// </summary>
    Task<IEnumerable<Perfil>> GetAllAtivosAsync();
}