using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdensServico;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Domain.Entities;
using Ofichina.Contracts.Responses.ItensServico;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para listar os itens de servico de uma ordem de servico.
/// </summary>
public sealed class GetItemServicosByOrdemServicoQueryHandler : IQueryHandler<GetItemServicosByOrdemServicoQuery, Result<IReadOnlyCollection<ItemServicoResponse>>>
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

    public async Task<Result<IReadOnlyCollection<ItemServicoResponse>>> HandleAsync(GetItemServicosByOrdemServicoQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(query.OrdemServicoId, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<IReadOnlyCollection<ItemServicoResponse>>("Ordem de serviço não encontrada.");

            var itens = await _itemServicoRepository.GetByOrdemServicoIdAsync(query.OrdemServicoId, cancellationToken, includeRelacionados: true);

            var resultado = itens
                .Where(item => !item.EstaExcluida())
                .Select(Mapear)
                .ToList();

            return Result.Success<IReadOnlyCollection<ItemServicoResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar itens de serviço. OrdemServicoId: {OrdemServicoId}.", query.OrdemServicoId);
            return Result.Failure<IReadOnlyCollection<ItemServicoResponse>>("Não foi possível obter os itens de serviço.");
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
                .Select(peca => MapearPeca(item.Id, peca))
                .ToList(),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            DeletedAt = item.DeletedAt
        };
    }

    private static OrdemServicoPecaResponse MapearPeca(Guid itemServicoId, ServicoPeca peca)
    {
        return new OrdemServicoPecaResponse
        {
            Id = peca.Id,
            PecaId = peca.PecaId,
            ItemServicoId = itemServicoId,
            ServicoId = peca.ServicoId,
            Descricao = peca.Peca?.Nome ?? string.Empty,
            Quantidade = peca.Quantidade,
            ValorUnitario = peca.Peca?.Valor ?? 0,
            ValorTotal = peca.ValorTotal,
            Utilizada = peca.Utilizada,
            DataUtilizacao = peca.DataUtilizacao
        };
    }
}

