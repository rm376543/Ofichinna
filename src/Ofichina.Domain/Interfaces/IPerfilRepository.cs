using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Interfaces;

/// <summary>
/// Interface específica para o repositório de Perfil.
/// </summary>
public interface IPerfilRepository : IRepository<Perfil>
{
    /// <summary>
    /// Busca um perfil pelo nome do perfil.
    /// </summary>
    Task<Perfil?> GetByNomeAsync(string nomePerfil);

    /// <summary>
    /// Lista todos os perfis ativos.
    /// </summary>
    Task<IEnumerable<Perfil>> GetAllAtivosAsync();
}