using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pecas.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pecas;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Pecas.Handlers;

/// <summary>
/// Handler para listar peças.
/// </summary>
public sealed class GetPecasQueryHandler : IQueryHandler<GetPecasQuery, Result<IReadOnlyCollection<PecaResponse>>>
{
    private readonly IRepository<Peca> _pecaRepository;
    private readonly ILogger<GetPecasQueryHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instância do handler de listagem de peças.
    /// </summary>
    public GetPecasQueryHandler(
        IRepository<Peca> pecaRepository,
        ILogger<GetPecasQueryHandler> logger)
    {
        _pecaRepository = pecaRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyCollection<PecaResponse>>> HandleAsync(GetPecasQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var pecas = await _pecaRepository.GetPagedAsync(query.Pagination, cancellationToken);

            var resultado = pecas.Items
                .Select(Mapear)
                .ToList();

            return Result.Success<IReadOnlyCollection<PecaResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar peças.");
            return Result.Failure<IReadOnlyCollection<PecaResponse>>("Não foi possível obter as peças.");
        }
    }

    private static PecaResponse Mapear(Peca peca)
    {
        return new PecaResponse
        {
            Id = peca.Id,
            Nome = peca.Nome,
            Descricao = peca.Descricao,
            Codigo = peca.Codigo,
            Valor = peca.Valor,
            QuantidadeEstoque = peca.QuantidadeEstoque,
            Ativo = !peca.EstaExcluida(),
            CreatedAt = peca.CreatedAt,
            UpdatedAt = peca.UpdatedAt,
            DeletedAt = peca.DeletedAt
        };
    }
}
