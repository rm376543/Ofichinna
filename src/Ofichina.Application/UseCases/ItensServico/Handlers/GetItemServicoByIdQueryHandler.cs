using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para obter um item de serviço por identificador.
/// </summary>
public sealed class GetItemServicoByIdQueryHandler
    : IQueryHandler<GetItemServicoByIdQuery, Result<OrdemServicoItensResponse>>
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

    public async Task<Result<OrdemServicoItensResponse>> HandleAsync(
        GetItemServicoByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(
                query.OrdemServicoId,
                cancellationToken);

            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<OrdemServicoItensResponse>(
                    "Ordem de serviço não encontrada.");

            var item = await _itemServicoRepository
                .GetByOrdemServicoIdAndItemServicoIdAsync(
                    query.OrdemServicoId,
                    query.Id,
                    cancellationToken,
                    includeRelacionados: true);

            if (item is null || item.EstaExcluida())
                return Result.Failure<OrdemServicoItensResponse>(
                    "Item de serviço não encontrado.");

            var response = new OrdemServicoItensResponse
            {
                OrdemServicoId = item.OrdemServicoId ?? Guid.Empty,
                Servicos =
                [
                    new ServicoItemResponse
                    {
                        ServicoId = item.ServicoId,
                        Descricao = item.Servico?.Nome ?? string.Empty,
                        ValorServico = item.Servico?.Valor ?? 0,

                        Pecas = item.PecaId.HasValue
                            ?
                            [
                                new PecaItemResponse
                                {
                                    PecaId = item.PecaId.Value,
                                    Descricao = item.Peca?.Nome ?? string.Empty,
                                    Quantidade = item.Quantidade,
                                    ValorUnitario = item.Peca?.Valor ?? 0,
                                    ValorTotal = (item.Peca?.Valor ?? 0) * item.Quantidade
                                }
                            ]
                            : [],

                        ValorTotal =
                            (item.Servico?.Valor ?? 0) +
                            ((item.Peca?.Valor ?? 0) * item.Quantidade)
                    }
                ]
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao obter item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.",
                query.OrdemServicoId,
                query.Id);

            return Result.Failure<OrdemServicoItensResponse>(
                "Não foi possível obter o item de serviço.");
        }
    }
}