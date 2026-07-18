using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pecas.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pecas;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Pecas.Handlers;

/// <summary>
/// Handler para obter peÃ§a por Id.
/// </summary>
public sealed class GetPecaByIdQueryHandler : IQueryHandler<GetPecaByIdQuery, Result<PecaResponse>>
{
    private readonly IRepository<Peca> _pecaRepository;
    private readonly ILogger<GetPecaByIdQueryHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instÃ¢ncia do handler de busca por Id.
    /// </summary>
    public GetPecaByIdQueryHandler(
        IRepository<Peca> pecaRepository,
        ILogger<GetPecaByIdQueryHandler> logger)
    {
        _pecaRepository = pecaRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<PecaResponse>> HandleAsync(GetPecaByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var peca = await _pecaRepository.GetByIdAsync(query.Id, cancellationToken);

            if (peca is null)
                return Result.Failure<PecaResponse>("PeÃ§a nÃ£o encontrada.");

            if (peca.EstaExcluida())
                return Result.Failure<PecaResponse>("PeÃ§a nÃ£o encontrada.");

            return Result.Success(Mapear(peca));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter peÃ§a com Id: {PecaId}", query.Id);
            return Result.Failure<PecaResponse>("NÃ£o foi possÃ­vel obter a peÃ§a.");
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
