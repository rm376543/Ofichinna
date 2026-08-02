using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Orcamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;
using Ofichina.Domain.Aggregates;

namespace Ofichina.Application.UseCases.Orcamentos.Handlers;

/// <summary>
/// Handler para obter orçamento por identificador.
/// </summary>
public sealed class GetOrcamentoByIdQueryHandler : IQueryHandler<GetOrcamentoByIdQuery, Result<OrcamentoResponse>>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly ILogger<GetOrcamentoByIdQueryHandler> _logger;

    public GetOrcamentoByIdQueryHandler(
        IOrcamentoRepository orcamentoRepository,
        ILogger<GetOrcamentoByIdQueryHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _logger = logger;
    }

    public async Task<Result<OrcamentoResponse>> HandleAsync(GetOrcamentoByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var orcamento = await _orcamentoRepository.GetByIdAsync(query.Id, includeItens: true, cancellationToken);
            if (orcamento is null || orcamento.EstaExcluida())
                return Result.Failure<OrcamentoResponse>("Orçamento não encontrado.");

            return Result.Success(Mapear(orcamento));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter orçamento por identificador. OrcamentoId: {OrcamentoId}", query.Id);
            return Result.Failure<OrcamentoResponse>("Não foi possível obter o orçamento.");
        }
    }

    private static OrcamentoResponse Mapear(Orcamento orcamento)
    {
        return new OrcamentoResponse
        {
            Id = orcamento.Id,
            PessoaId = orcamento.PessoaId,
            VeiculoId = orcamento.VeiculoId,
            MecanicoDiagnosticoId = orcamento.MecanicoDiagnosticoId,
            ResponsavelId = orcamento.ResponsavelId,
            DataValidade = orcamento.DataValidade,
            Desconto = orcamento.Desconto,
            Observacoes = orcamento.Observacoes,
            Status = orcamento.Status.ToString(),
            DataCriacao = orcamento.DataCriacao,
            CreatedAt = orcamento.CreatedAt,
            UpdatedAt = orcamento.UpdatedAt,
            DeletedAt = orcamento.DeletedAt,
            Checklist = orcamento.Checklist is null
                ? null
                : new ChecklistResponse
                {
                    Id = orcamento.Checklist.Id,
                    OrcamentoId = orcamento.Checklist.OrcamentoId,
                    HodometroEntrada = orcamento.Checklist.HodometroEntrada,
                    ItensVerificados = orcamento.Checklist.ItensVerificados,
                    Observacoes = orcamento.Checklist.Observacoes,
                    CreatedAt = orcamento.Checklist.CreatedAt,
                    UpdatedAt = orcamento.Checklist.UpdatedAt,
                    DeletedAt = orcamento.Checklist.DeletedAt
                },
            ItensPrevistos = orcamento.ItensPrevistos
                .Where(x => !x.EstaExcluida())
                .Select(x => new OrcamentoItemResponse
                {
                    Id = x.Id,
                    OrcamentoId = x.OrcamentoId,
                    ServicoId = x.ServicoId == Guid.Empty ? null : x.ServicoId,
                    PecaId = x.PecaId == Guid.Empty ? null : x.PecaId,
                    Quantidade = x.Quantidade,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt
                })
                .ToList()
        };
    }
}
