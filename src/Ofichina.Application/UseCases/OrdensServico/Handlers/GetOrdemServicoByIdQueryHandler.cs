using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Contracts.Responses.OrdensServico;
using Ofichina.Domain.Aggregates;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para obter uma ordem de serviço por identificador.
/// </summary>
public sealed class GetOrdemServicoByIdQueryHandler
    : IQueryHandler<GetOrdemServicoByIdQuery, Result<OrdemServicoResponse>>
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

    public async Task<Result<OrdemServicoResponse>> HandleAsync(
        GetOrdemServicoByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(
                query.Id,
                includeItens: true,
                cancellationToken);

            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<OrdemServicoResponse>(
                    "Ordem de serviço não encontrada.");

            return Result.Success(Mapear(ordemServico));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao obter ordem de serviço por identificador. OrdemServicoId: {OrdemServicoId}",
                query.Id);

            return Result.Failure<OrdemServicoResponse>(
                "Não foi possível obter a ordem de serviço.");
        }
    }

    private static OrdemServicoResponse Mapear(OrdemServico ordemServico)
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

            CreatedAt = ordemServico.CreatedAt,
            UpdatedAt = ordemServico.UpdatedAt,
            DeletedAt = ordemServico.DeletedAt,

            Servicos = ordemServico.Servicos
                .Where(x => !x.EstaExcluida())
                .GroupBy(x => new
                {
                    x.ServicoId,
                    Nome = x.Servico?.Nome ?? string.Empty,
                    Valor = x.Servico?.Valor ?? 0
                })
                .Select(g => new OrdemServicoItensResponse
                {
                    OrdemServicoId = ordemServico.Id,

                    Servicos =
                    [
                        new ServicoItemResponse
                        {
                            ServicoId = g.Key.ServicoId,
                            Descricao = g.Key.Nome,
                            ValorServico = g.Key.Valor,

                            Pecas = g
                                .Select(p => new PecaItemResponse
                                {
                                    PecaId = p.PecaId,
                                    Descricao = p.Peca?.Nome ?? string.Empty,
                                    Quantidade = p.Quantidade,
                                    ValorUnitario = p.Peca?.Valor ?? 0,
                                    ValorTotal = (p.Peca?.Valor ?? 0) * p.Quantidade
                                })
                                .ToList(),

                            ValorTotal = g.Key.Valor +
                                         g.Sum(p => (p.Peca?.Valor ?? 0) * p.Quantidade)
                        }
                    ]
                })
                .ToList()
        };
    }
}