using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pecas.Mappings;
using Ofichina.Application.UseCases.Pecas.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pecas;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Pecas.Handlers;

/// <summary>
/// Handler para obter peça por Id.
/// </summary>
public sealed class GetPecaByIdQueryHandler : IQueryHandler<GetPecaByIdQuery, Result<PecaResponse>>
{
    private readonly IRepository<Peca> _pecaRepository;
    private readonly ILogger<GetPecaByIdQueryHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instância do handler de busca por Id.
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
            var peca = await _pecaRepository.GetByIdAsync(query.PecaId, cancellationToken);

            if (peca is null)
                return Result.Failure<PecaResponse>("Peça não encontrada.");

            if (peca.EstaExcluida())
                return Result.Failure<PecaResponse>("Peça excluida ou não encontrada.");

            return Result.Success(peca.ToResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter peça com Id: {PecaId}", query.PecaId);
            return Result.Failure<PecaResponse>("Não foi possível obter a peça.");
        }
    }

}
