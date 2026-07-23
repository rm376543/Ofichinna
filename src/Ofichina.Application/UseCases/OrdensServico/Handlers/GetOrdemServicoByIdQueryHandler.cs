using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.ItensServico;
using Ofichina.Contracts.Responses.OrdensServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para obter uma ordem de serviço por identificador.
/// </summary>
public sealed class GetOrdemServicoByIdQueryHandler : IQueryHandler<GetOrdemServicoByIdQuery, Result<OrdemServicoResponse>>
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

    private Task<Dictionary<Guid, Servico>> CarregarServicosAsync(
        IEnumerable<ItemServico> itens,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new Dictionary<Guid, Servico>());
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
        ItemServico item,
        IReadOnlyDictionary<Guid, Servico> servicosPorId)
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
            Descricao = peca.Descricao,
            Quantidade = peca.Quantidade,
            ValorUnitario = peca.ValorUnitario,
            ValorTotal = peca.ValorTotal,
            Utilizada = peca.Utilizada,
            DataUtilizacao = peca.DataUtilizacao
        };
    }
}

