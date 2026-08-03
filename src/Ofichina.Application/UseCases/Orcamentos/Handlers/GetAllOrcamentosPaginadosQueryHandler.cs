using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Orcamentos.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;

namespace Ofichina.Application.UseCases.Orcamentos.Handlers;

/// <summary>
/// Handler para listar orçamentos.
/// </summary>
public sealed class GetAllOrcamentosPaginadosQueryHandler : IQueryHandler<GetAllOrcamentosPaginadosQuery, Result<PagedResponse<OrcamentoSimplesResponse>>>
{
    private readonly IOrcamentoService _orcamentoService;
    private readonly ILogger<GetAllOrcamentosPaginadosQueryHandler> _logger;

    public GetAllOrcamentosPaginadosQueryHandler(
        IOrcamentoService orcamentoService,
        ILogger<GetAllOrcamentosPaginadosQueryHandler> logger)
    {
        _orcamentoService = orcamentoService;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<OrcamentoSimplesResponse>>> HandleAsync(GetAllOrcamentosPaginadosQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var resultados = await _orcamentoService.GetAllPagedAsync(query.Pagination, cancellationToken);
            return Result.Success(resultados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar orçamentos.");
            return Result.Failure<PagedResponse<OrcamentoSimplesResponse>>("Não foi possível obter os orçamentos.");
        }
    }
}
