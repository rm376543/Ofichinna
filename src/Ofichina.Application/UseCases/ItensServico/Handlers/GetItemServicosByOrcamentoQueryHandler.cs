using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para listar itens de serviço de um orçamento.
/// </summary>
public sealed class GetItemServicosByOrcamentoQueryHandler : IQueryHandler<GetItemServicosByOrcamentoQuery, Result<IReadOnlyCollection<OrcamentoItemResponse>>>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly ILogger<GetItemServicosByOrcamentoQueryHandler> _logger;

    public GetItemServicosByOrcamentoQueryHandler(
        IOrcamentoRepository orcamentoRepository,
        IItemServicoRepository itemServicoRepository,
        ILogger<GetItemServicosByOrcamentoQueryHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _itemServicoRepository = itemServicoRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<OrcamentoItemResponse>>> HandleAsync(GetItemServicosByOrcamentoQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var orcamento = await _orcamentoRepository.GetByIdAsync(query.OrcamentoId, cancellationToken);
            if (orcamento is null || orcamento.EstaExcluida())
                return Result.Failure<IReadOnlyCollection<OrcamentoItemResponse>>("Orçamento não encontrado.");

            var itens = await _itemServicoRepository.GetByOrcamentoIdAsync(query.OrcamentoId, cancellationToken, includeRelacionados: true);

            var response = new OrcamentoItemResponse
            {
                OrcamentoId = query.OrcamentoId,
                Servicos = itens
                    .Where(x => !x.EstaExcluida())
                    .GroupBy(x => new
                    {
                        x.ServicoId,
                        Nome = x.Servico?.Nome ?? string.Empty,
                        Valor = x.Servico?.Valor ?? 0m
                    })
                    .Select(servico => new ServicoItemResponse
                    {
                        ServicoId = servico.Key.ServicoId,
                        Descricao = servico.Key.Nome,
                        ValorServico = servico.Key.Valor,
                        Pecas = servico
                            .Where(p => p.PecaId.HasValue)
                            .Select(p => new PecaItemResponse
                            {
                                PecaId = p.PecaId!.Value,
                                Descricao = p.Peca?.Nome ?? string.Empty,
                                Quantidade = p.Quantidade,
                                ValorUnitario = p.Peca?.Valor ?? 0m,
                                ValorTotal = (p.Peca?.Valor ?? 0m) * p.Quantidade
                            })
                            .ToList(),
                        ValorTotal = servico.Key.Valor + servico.Sum(p => (p.Peca?.Valor ?? 0m) * p.Quantidade)
                    })
                    .ToList()
            };

            return Result.Success<IReadOnlyCollection<OrcamentoItemResponse>>([response]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar itens de serviço do orçamento. OrcamentoId: {OrcamentoId}.", query.OrcamentoId);
            return Result.Failure<IReadOnlyCollection<OrcamentoItemResponse>>("Não foi possível obter os itens de serviço do orçamento.");
        }
    }
}
