using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para obter uma ordem de serviÃ§o por identificador.
/// </summary>
public sealed class GetOrdemServicoByIdQueryHandler : IQueryHandler<GetOrdemServicoByIdQuery, Result<OrdemServicoResponse>>
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly ILogger<GetOrdemServicoByIdQueryHandler> _logger;

    public GetOrdemServicoByIdQueryHandler(
        IRepository<OrdemServico> ordemServicoRepository,
        ILogger<GetOrdemServicoByIdQueryHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _logger = logger;
    }

    public async Task<Result<OrdemServicoResponse>> HandleAsync(GetOrdemServicoByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(query.Id, cancellationToken);

            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<OrdemServicoResponse>("Ordem de serviÃ§o nÃ£o encontrada.");

            return Result.Success(Mapear(ordemServico));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter ordem de serviÃ§o por identificador. OrdemServicoId: {OrdemServicoId}", query.Id);
            return Result.Failure<OrdemServicoResponse>("NÃ£o foi possÃ­vel obter a ordem de serviÃ§o.");
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

