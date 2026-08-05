using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para listar os itens de servico de uma ordem de servico.
/// </summary>
public sealed class GetItemServicosByOrdemServicoQueryHandler : IQueryHandler<GetItemServicosByOrdemServicoQuery, Result<IReadOnlyCollection<OrdemServicoItensResponse>>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly ILogger<GetItemServicosByOrdemServicoQueryHandler> _logger;

    public GetItemServicosByOrdemServicoQueryHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        ILogger<GetItemServicosByOrdemServicoQueryHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _itemServicoRepository = itemServicoRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<OrdemServicoItensResponse>>> HandleAsync(GetItemServicosByOrdemServicoQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(query.OrdemServicoId, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<IReadOnlyCollection<OrdemServicoItensResponse>>("Ordem de serviço não encontrada.");

            var itens = await _itemServicoRepository
                .GetByOrdemServicoIdAsync(
                    query.OrdemServicoId,
                    cancellationToken,
                    includeRelacionados: true);

            var response = new OrdemServicoItensResponse
            {
                OrdemServicoId = query.OrdemServicoId,
                Servicos = itens
                    .Where(x => !x.EstaExcluida())
                    .GroupBy(x => new
                    {
                        x.ServicoId,
                        Nome = x.Servico!.Nome,
                        Valor = x.Servico!.Valor
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
                                Descricao = p.Peca?.Nome ?? "",
                                Quantidade = p.Quantidade,
                                ValorUnitario = p.Peca?.Valor ?? 0,
                                ValorTotal = (p.Peca?.Valor ?? 0) * p.Quantidade
                            })
                            .ToList(),

                        ValorTotal =
                            servico.Key.Valor +
                            servico.Sum(p => (p.Peca?.Valor ?? 0) * p.Quantidade)
                    })
                    .ToList()
            };

            return Result.Success<IReadOnlyCollection<OrdemServicoItensResponse>>(new List<OrdemServicoItensResponse> { response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar itens de serviço. OrdemServicoId: {OrdemServicoId}.", query.OrdemServicoId);
            return Result.Failure<IReadOnlyCollection<OrdemServicoItensResponse>>("Não foi possível obter os itens de serviço.");
        }
    }
}

