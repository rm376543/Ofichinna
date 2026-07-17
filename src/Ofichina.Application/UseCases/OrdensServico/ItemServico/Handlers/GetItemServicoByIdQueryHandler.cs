using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.ItemServico.Handlers;

/// <summary>
/// Handler para obter um item de serviço por identificador.
/// </summary>
public sealed class GetItemServicoByIdQueryHandler : IQueryHandler<GetItemServicoByIdQuery, Result<ItemServicoResponse>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly ILogger<GetItemServicoByIdQueryHandler> _logger;

    public GetItemServicoByIdQueryHandler(
        IOrdemServicoRepository ordemServicoRepository,
        ILogger<GetItemServicoByIdQueryHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _logger = logger;
    }

    public async Task<Result<ItemServicoResponse>> HandleAsync(GetItemServicoByIdQuery query)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(query.OrdemServicoId, includeItens: true);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<ItemServicoResponse>("Ordem de serviço não encontrada.");

            var item = ordemServico.ObterServico(query.Id);
            if (item is null || item.EstaExcluida())
                return Result.Failure<ItemServicoResponse>("Item de serviço não encontrado.");

            return Result.Success(Mapear(item));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", query.OrdemServicoId, query.Id);
            return Result.Failure<ItemServicoResponse>("Não foi possível obter o item de serviço.");
        }
    }

    private static ItemServicoResponse Mapear(Ofichina.Domain.Entities.ItemServico item)
    {
        return new ItemServicoResponse
        {
            Id = item.Id,
            ServicoId = item.ServicoId,
            OrdemServicoId = item.OrdemServicoId,
            Descricao = item.Descricao,
            Valor = item.Valor,
            ValorTotal = item.ValorTotal,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            DeletedAt = item.DeletedAt
        };
    }
}
