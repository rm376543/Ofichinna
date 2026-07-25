using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdensServico;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Contracts.Responses.ItensServico;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para obter um item de serviço por identificador.
/// </summary>
public sealed class GetItemServicoByIdQueryHandler : IQueryHandler<GetItemServicoByIdQuery, Result<ItemServicoResponse>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly ILogger<GetItemServicoByIdQueryHandler> _logger;

    public GetItemServicoByIdQueryHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        ILogger<GetItemServicoByIdQueryHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _itemServicoRepository = itemServicoRepository;
        _logger = logger;
    }

    public async Task<Result<ItemServicoResponse>> HandleAsync(GetItemServicoByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(query.OrdemServicoId, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<ItemServicoResponse>("Ordem de serviço não encontrada.");

            var item = await _itemServicoRepository.GetByOrdemServicoIdAndItemServicoIdAsync(query.OrdemServicoId, query.Id, cancellationToken, includeRelacionados: true);
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

    private static ItemServicoResponse Mapear(Domain.Entities.ItemServico item)
    {
        return new ItemServicoResponse
        {
            Id = item.Id,
            ServicoId = Guid.Empty,  // Não há mais vínculo direto com serviço
            OrdemServicoId = item.OrdemServicoId,
            Descricao = item.Descricao,
            Valor = item.Valor,
            ValorTotal = item.ValorTotal,
            Pecas = item.Pecas
                .Select(peca => new OrdemServicoPecaResponse
                {
                    Id = peca.Id,
                    PecaId = peca.PecaId,
                    ItemServicoId = item.Id,
                    ServicoId = peca.ServicoId,
                    Descricao = peca.Peca?.Nome ?? string.Empty,
                    Quantidade = peca.Quantidade,
                    ValorUnitario = peca.Peca?.Valor ?? 0,
                    ValorTotal = peca.ValorTotal,
                    Utilizada = peca.Utilizada,
                    DataUtilizacao = peca.DataUtilizacao
                })
                .ToList(),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            DeletedAt = item.DeletedAt
        };
    }
}

