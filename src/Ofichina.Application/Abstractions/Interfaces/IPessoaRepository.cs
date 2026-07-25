using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IPessoaRepository : IRepository<Pessoa>
{
    /// <summary>
    /// Obtém uma pessoa pelo ID do usuário associado.
    /// </summary>
    /// <param name="usuarioId">ID do usuário associado à pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>A pessoa associada ao ID do usuário, ou null se não encontrada.</returns>
    Task<Pessoa?> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém uma pessoa pelo ID da pessoa, incluindo os veículos associados.
    /// </summary>
    /// <param name="pessoaId">ID da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>A pessoa associada ao ID informado, incluindo os veículos, ou null se não encontrada.</returns>
    Task<Pessoa?> GetByIdWithVeiculosAsync(Guid pessoaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pesquisa todas pessoas e mostra paginadas de acordo com a paginação informada.
    /// </summary>
    /// <param name="pagination">Informações de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Uma resposta paginada contendo as pessoas.</returns>
    Task<PagedResponse<Pessoa>> GetAllPessoasPaginadasAsync(Pagination pagination, CancellationToken cancellationToken = default);
}

