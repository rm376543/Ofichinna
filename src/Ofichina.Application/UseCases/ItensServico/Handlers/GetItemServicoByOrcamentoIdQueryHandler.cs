using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para obter um item de serviço específico de um orçamento.
/// </summary>
public sealed class GetItemServicoByOrcamentoIdQueryHandler : IQueryHandler<GetItemServicoByOrcamentoIdQuery, Result<OrcamentoItemResponse>>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly ILogger<GetItemServicoByOrcamentoIdQueryHandler> _logger;

    public GetItemServicoByOrcamentoIdQueryHandler(
        IOrcamentoRepository orcamentoRepository,
        IItemServicoRepository itemServicoRepository,
        ILogger<GetItemServicoByOrcamentoIdQueryHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _itemServicoRepository = itemServicoRepository;
        _logger = logger;
    }

    public async Task<Result<OrcamentoItemResponse>> HandleAsync(GetItemServicoByOrcamentoIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var orcamento = await _orcamentoRepository.GetByIdAsync(query.OrcamentoId, cancellationToken);
            if (orcamento is null || orcamento.EstaExcluida())
                return Result.Failure<OrcamentoItemResponse>("Orçamento não encontrado.");

            var item = await _itemServicoRepository.GetByOrcamentoIdAndItemServicoIdAsync(query.OrcamentoId, query.ItemServicoId, cancellationToken, includeRelacionados: true);
            if (item is null || item.EstaExcluida())
                return Result.Failure<OrcamentoItemResponse>("Item de serviço não encontrado.");

            var response = new OrcamentoItemResponse
            {
                OrcamentoId = item.OrcamentoId ?? Guid.Empty,
                Servicos =
                [
                    new ServicoItemResponse
                    {
                        ServicoId = item.ServicoId,
                        Descricao = item.Servico?.Nome ?? string.Empty,
                        ValorServico = item.Servico?.Valor ?? 0m,
                        Pecas = item.PecaId.HasValue
                            ?
                            [
                                new PecaItemResponse
                                {
                                    PecaId = item.PecaId.Value,
                                    Descricao = item.Peca?.Nome ?? string.Empty,
                                    Quantidade = item.Quantidade,
                                    ValorUnitario = item.Peca?.Valor ?? 0m,
                                    ValorTotal = (item.Peca?.Valor ?? 0m) * item.Quantidade
                                }
                            ]
                            : [],
                        ValorTotal = (item.Servico?.Valor ?? 0m) + ((item.Peca?.Valor ?? 0m) * item.Quantidade)
                    }
                ]
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter item de serviço do orçamento. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", query.OrcamentoId, query.ItemServicoId);
            return Result.Failure<OrcamentoItemResponse>("Não foi possível obter o item de serviço do orçamento.");
        }
    }
}
