using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Orcamentos.Mappings;
using Ofichina.Application.UseCases.Orcamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;

namespace Ofichina.Application.UseCases.Orcamentos.Handlers;

/// <summary>
/// Handler para obter orçamento por identificador.
/// </summary>
public sealed class GetOrcamentoByIdQueryHandler : IQueryHandler<GetOrcamentoByIdQuery, Result<OrcamentoResponse>>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly ILogger<GetOrcamentoByIdQueryHandler> _logger;

    public GetOrcamentoByIdQueryHandler(
        IOrcamentoRepository orcamentoRepository,
        ILogger<GetOrcamentoByIdQueryHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _logger = logger;
    }

    public async Task<Result<OrcamentoResponse>> HandleAsync(GetOrcamentoByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var orcamento = await _orcamentoRepository.GetByIdAsync(query.OrcamentoId, includeItens: true, cancellationToken);

            if (orcamento is null || orcamento.EstaExcluida())
            {
                return Result.Failure<OrcamentoResponse>("Orçamento não encontrado.");
            }

            return Result.Success(orcamento.ToResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter orçamento por identificador. OrcamentoId: {OrcamentoId}", query.OrcamentoId);
            return Result.Failure<OrcamentoResponse>("Não foi possível obter o orçamento.");
        }
    }
}
