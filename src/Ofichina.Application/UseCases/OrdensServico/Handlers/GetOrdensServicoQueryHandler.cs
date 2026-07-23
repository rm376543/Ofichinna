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
/// Handler para listar ordens de serviço.
/// </summary>
public sealed class GetOrdensServicoQueryHandler : IQueryHandler<GetOrdensServicoQuery, Result<IReadOnlyCollection<OrdemServicoResponse>>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly ILogger<GetOrdensServicoQueryHandler> _logger;

    public GetOrdensServicoQueryHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IServicoRepository servicoRepository,
        ILogger<GetOrdensServicoQueryHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _servicoRepository = servicoRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<OrdemServicoResponse>>> HandleAsync(GetOrdensServicoQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ordensServico = await _ordemServicoRepository.GetAllAsync(includeItens: true, cancellationToken);

            var servicosPorId = await CarregarServicosAsync(ordensServico, cancellationToken);

            var resultado = ordensServico
                .Skip(query.Pagination.GetSkip())
                .Take(query.Pagination.PageSize)
                .Select(ordem => Mapear(ordem, servicosPorId))
                .ToList();

            return Result.Success<IReadOnlyCollection<OrdemServicoResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar ordens de serviço.");
            return Result.Failure<IReadOnlyCollection<OrdemServicoResponse>>("Não foi possível obter as ordens de serviço.");
        }
    }

    private Task<Dictionary<Guid, Servico>> CarregarServicosAsync(
        IEnumerable<OrdemServico> ordensServico,
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
            ServicoId = Guid.Empty,
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
                    Descricao = peca.Descricao,
                    Quantidade = peca.Quantidade,
                    ValorUnitario = peca.ValorUnitario,
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
