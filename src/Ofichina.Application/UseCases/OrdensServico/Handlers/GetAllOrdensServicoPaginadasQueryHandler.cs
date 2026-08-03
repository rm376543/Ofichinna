using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para listar ordens de serviço.
/// </summary>
public sealed class GetAllOrdensServicoPaginadasQueryHandler : IQueryHandler<GetAllOrdensServicoPaginadasQuery, Result<PagedResponse<OrdemServicoSimplesResponse>>>
{
    private readonly IOrdemServicoService _ordemServicoReadService;
    private readonly ILogger<GetAllOrdensServicoPaginadasQueryHandler> _logger;

    public GetAllOrdensServicoPaginadasQueryHandler(
        IOrdemServicoService ordemServicoReadService,
        ILogger<GetAllOrdensServicoPaginadasQueryHandler> logger)
    {
        _ordemServicoReadService = ordemServicoReadService;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<OrdemServicoSimplesResponse>>> HandleAsync(
        GetAllOrdensServicoPaginadasQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Listando ordens de serviço com paginação");
            var resultados = await _ordemServicoReadService.GetAllPagedAsync(query.Pagination, cancellationToken);

            _logger.LogInformation("Ordens de serviço listadas com sucesso");
            return Result.Success(resultados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar ordens de serviço.");
            return Result.Failure<PagedResponse<OrdemServicoSimplesResponse>>("Não foi possível obter as ordens de serviço.");
        }
    }
}
