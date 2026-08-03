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
    /// Obtém uma pessoa pelo ID da pessoa, incluindo os veículos associados quando solicitado.
    /// </summary>
    /// <param name="pessoaId">ID da pessoa.</param>
    /// <param name="includeVeiculos">Indica se os veículos associados devem ser carregados.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>A pessoa associada ao ID informado, incluindo os veículos quando solicitado, ou null se não encontrada.</returns>
    Task<Pessoa?> GetByIdAsync(Guid pessoaId, bool includeVeiculos = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém várias pessoas pelos identificadores informados.
    /// </summary>
    /// <param name="pessoaIds">Identificadores das pessoas.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Coleção de pessoas encontradas.</returns>
    Task<IReadOnlyCollection<Pessoa>> GetByIdsAsync(IEnumerable<Guid> pessoaIds, CancellationToken cancellationToken = default);

}

