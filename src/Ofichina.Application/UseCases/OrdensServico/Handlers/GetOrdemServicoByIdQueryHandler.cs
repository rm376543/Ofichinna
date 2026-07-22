using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para obter uma ordem de serviço por identificador.
/// </summary>
public sealed class GetOrdemServicoByIdQueryHandler : IQueryHandler<GetOrdemServicoByIdQuery, Result<OrdemServicoResponse>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly ILogger<GetOrdemServicoByIdQueryHandler> _logger;

    public GetOrdemServicoByIdQueryHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IServicoRepository servicoRepository,
        ILogger<GetOrdemServicoByIdQueryHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _servicoRepository = servicoRepository;
        _logger = logger;
    }

    public async Task<Result<OrdemServicoResponse>> HandleAsync(GetOrdemServicoByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(query.Id, includeItens: true, cancellationToken);

            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<OrdemServicoResponse>("Ordem de serviço não encontrada.");

            var servicosPorId = await CarregarServicosAsync(ordemServico.Servicos, cancellationToken);

            return Result.Success(Mapear(ordemServico, servicosPorId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter ordem de serviço por identificador. OrdemServicoId: {OrdemServicoId}", query.Id);
            return Result.Failure<OrdemServicoResponse>("Não foi possível obter a ordem de serviço.");
        }
    }

    private async Task<Dictionary<Guid, Servico>> CarregarServicosAsync(
        IEnumerable<Ofichina.Domain.Entities.ItemServico> itens,
        CancellationToken cancellationToken)
    {
        var ids = itens
            .Where(item => item.PecaServico is not null)
            .Select(item => item.PecaServico!.ServicoId)
            .Distinct()
            .ToList();

        var resultados = await Task.WhenAll(ids.Select(async id =>
        {
            var servico = await _servicoRepository.GetByIdAsync(id, includePecas: true, cancellationToken);
            return (id, servico);
        }));

        return resultados
            .Where(x => x.servico is not null && !x.servico.EstaExcluida())
            .ToDictionary(x => x.id, x => x.servico!);
    }

    private static OrdemServicoResponse Mapear(
        OrdemServico ordemServico,
        IReadOnlyDictionary<Guid, Servico> servicosPorId)
    {
        return new OrdemServicoResponse
        {
            Id = ordemServico.Id,
            PessoaId = ordemServico.PessoaId,
            VeiculoId = ordemServico.VeiculoId,
            FuncionarioId = ordemServico.FuncionarioId,
            HodometroEntrada = ordemServico.HodometroEntrada,
            ProblemaRelatado = ordemServico.ProblemaRelatado,
            Status = ordemServico.Status.ToString(),
            DataAbertura = ordemServico.DataAbertura,
            DataFinalizacao = ordemServico.DataFinalizacao,
            Observacao = ordemServico.Observacao,
            ValorTotal = ordemServico.ValorTotal,
            Servicos = ordemServico.Servicos
                .Where(item => !item.EstaExcluida())
                .Select(item => MapearServico(item, servicosPorId))
                .ToList(),
            CreatedAt = ordemServico.CreatedAt,
            UpdatedAt = ordemServico.UpdatedAt,
            DeletedAt = ordemServico.DeletedAt
        };
    }

    private static ItemServicoResponse MapearServico(
        Ofichina.Domain.Entities.ItemServico item,
        IReadOnlyDictionary<Guid, Servico> servicosPorId)
    {
        var servico = item.PecaServico is null
            ? null
            : servicosPorId.GetValueOrDefault(item.PecaServico.ServicoId);

        return new ItemServicoResponse
        {
            Id = item.Id,
            ServicoId = item.ServicoId,
            OrdemServicoId = item.OrdemServicoId,
            Descricao = item.Descricao,
            Valor = item.Valor,
            ValorTotal = item.ValorTotal,
            Pecas = servico?.Pecas
                .Where(peca => !peca.EstaExcluida())
                .Select(peca => MapearPeca(item.Id, peca))
                .ToList() ?? [],
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            DeletedAt = item.DeletedAt
        };
    }

    private static OrdemServicoPecaResponse MapearPeca(Guid itemServicoId, Ofichina.Domain.Entities.PecaServico peca)
    {
        return new OrdemServicoPecaResponse
        {
            Id = peca.Id,
            PecaId = peca.PecaId,
            ItemServicoId = itemServicoId,
            ServicoId = peca.ServicoId,
            Descricao = peca.Descricao,
            Quantidade = peca.Quantidade,
            ValorUnitario = peca.ValorUnitario,
            ValorTotal = peca.ValorTotal,
            Utilizada = peca.Utilizada,
            DataUtilizacao = peca.DataUtilizacao,
            CreatedAt = peca.CreatedAt,
            UpdatedAt = peca.UpdatedAt,
            DeletedAt = peca.DeletedAt
        };
    }
}

