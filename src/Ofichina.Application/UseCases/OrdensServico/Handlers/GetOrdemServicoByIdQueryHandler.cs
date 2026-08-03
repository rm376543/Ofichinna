using Ofichina.Application.Abstractions;
using Ofichina.Application.Extensions;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Contracts.Responses.OrdensServico;
using Ofichina.Domain.Aggregates;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para obter uma ordem de serviço por identificador.
/// </summary>
public sealed class GetOrdemServicoByIdQueryHandler
    : IQueryHandler<GetOrdemServicoByIdQuery, Result<OrdemServicoResponse>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly ILogger<GetOrdemServicoByIdQueryHandler> _logger;

    public GetOrdemServicoByIdQueryHandler(
        IOrdemServicoRepository ordemServicoRepository,
        ILogger<GetOrdemServicoByIdQueryHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _logger = logger;
    }

    public async Task<Result<OrdemServicoResponse>> HandleAsync(
        GetOrdemServicoByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(
                query.Id,
                includeItens: true,
                cancellationToken);

            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<OrdemServicoResponse>(
                    "Ordem de serviço não encontrada.");

            return Result.Success(ordemServico.ToResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao obter ordem de serviço por identificador. OrdemServicoId: {OrdemServicoId}",
                query.Id);

            return Result.Failure<OrdemServicoResponse>(
                "Não foi possível obter a ordem de serviço.");
        }
    }
}