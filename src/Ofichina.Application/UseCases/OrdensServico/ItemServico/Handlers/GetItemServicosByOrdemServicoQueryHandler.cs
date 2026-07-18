using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.ItemServico.Handlers;

/// <summary>
/// Handler para listar os itens de serviÃ§o de uma ordem de serviÃ§o.
/// </summary>
public sealed class GetItemServicosByOrdemServicoQueryHandler : IQueryHandler<GetItemServicosByOrdemServicoQuery, Result<IReadOnlyCollection<ItemServicoResponse>>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly ILogger<GetItemServicosByOrdemServicoQueryHandler> _logger;

    public GetItemServicosByOrdemServicoQueryHandler(
        IOrdemServicoRepository ordemServicoRepository,
        ILogger<GetItemServicosByOrdemServicoQueryHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<ItemServicoResponse>>> HandleAsync(GetItemServicosByOrdemServicoQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(query.OrdemServicoId, includeItens: true, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<IReadOnlyCollection<ItemServicoResponse>>("Ordem de serviÃ§o nÃ£o encontrada.");

            var resultado = ordemServico.Servicos
                .Where(item => !item.EstaExcluida())
                .Select(Mapear)
                .ToList();

            return Result.Success<IReadOnlyCollection<ItemServicoResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar itens de serviÃ§o. OrdemServicoId: {OrdemServicoId}.", query.OrdemServicoId);
            return Result.Failure<IReadOnlyCollection<ItemServicoResponse>>("NÃ£o foi possÃ­vel obter os itens de serviÃ§o.");
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

