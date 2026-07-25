using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IPermissaoRepository : IRepository<Permissao>
{
    /// <summary>
    /// Busca uma permissão pelo código.
    /// </summary>
    /// <param name="codigo">Código da permissão.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>A permissão encontrada ou null se não existir.</returns>
    Task<Permissao?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todas as permissões de forma paginada.
    /// </summary>
    /// <param name="pagination">Informações de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Uma lista paginada de permissões.</returns>
    Task<PagedResponse<Permissao>> GetAllPermissoesPaginadasAsync(Pagination pagination, CancellationToken cancellationToken = default);
}
