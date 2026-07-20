using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Interface específica para o repositório de Perfil.
/// </summary>
public interface IPerfilRepository : IRepository<Perfil>
{
    /// <summary>
    /// Busca um perfil pelo nome do perfil.
    /// </summary>
    Task<Perfil?> GetByNomeAsync(string nomePerfil, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todos os perfis ativos.
    /// </summary>
    Task<IEnumerable<Perfil>> GetAllAtivosAsync(CancellationToken cancellationToken = default);
}