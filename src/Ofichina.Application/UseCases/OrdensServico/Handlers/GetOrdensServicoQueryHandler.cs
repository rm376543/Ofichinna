using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para listar ordens de serviço.
/// </summary>
public sealed class GetOrdensServicoQueryHandler : IQueryHandler<GetOrdensServicoQuery, Result<IReadOnlyCollection<OrdemServicoResponse>>>
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly ILogger<GetOrdensServicoQueryHandler> _logger;

    public GetOrdensServicoQueryHandler(
        IRepository<OrdemServico> ordemServicoRepository,
        ILogger<GetOrdensServicoQueryHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<OrdemServicoResponse>>> HandleAsync(GetOrdensServicoQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ordensServico = await _ordemServicoRepository.GetPagedAsync(query.Pagination, cancellationToken);

            var resultado = ordensServico.Items
                .Select(Mapear)
                .ToList();

            return Result.Success<IReadOnlyCollection<OrdemServicoResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar ordens de serviço.");
            return Result.Failure<IReadOnlyCollection<OrdemServicoResponse>>("Não foi possível obter as ordens de serviço.");
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
            Status = ordemServico.Status.ToString(),
            DataAbertura = ordemServico.DataAbertura,
            DataFinalizacao = ordemServico.DataFinalizacao,
            Observacao = ordemServico.Observacao,
            ValorTotal = ordemServico.ValorTotal,
            CreatedAt = ordemServico.CreatedAt,
            UpdatedAt = ordemServico.UpdatedAt,
            DeletedAt = ordemServico.DeletedAt
        };
    }
}
