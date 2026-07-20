using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Authentication.Abstractions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para listar agendamentos do usuário autenticado.
/// </summary>
public sealed class GetAgendamentosQueryHandler : IQueryHandler<GetAgendamentosQuery, Result<IReadOnlyCollection<AgendamentoResponse>>>
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly ILogger<GetAgendamentosQueryHandler> _logger;

    public GetAgendamentosQueryHandler(
        IAgendamentoRepository agendamentoRepository,
        IPessoaRepository pessoaRepository,
        ILogger<GetAgendamentosQueryHandler> logger)
    {
        _agendamentoRepository = agendamentoRepository;
        _pessoaRepository = pessoaRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<AgendamentoResponse>>> HandleAsync(GetAgendamentosQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(query.PessoaId, cancellationToken);

            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<IReadOnlyCollection<AgendamentoResponse>>("Pessoa não encontrada.");

            var agendamentos = await _agendamentoRepository.GetAllAsync(cancellationToken);

            var resultado = agendamentos
                .Where(x => !x.EstaExcluida() && x.ClientePessoaId == query.PessoaId)
                .Select(Mapear)
                .ToList();

            return Result.Success<IReadOnlyCollection<AgendamentoResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar agendamentos.");
            return Result.Failure<IReadOnlyCollection<AgendamentoResponse>>("Não foi possível obter os agendamentos.");
        }
    }

    private static AgendamentoResponse Mapear(Agendamento agendamento)
    {
        return new AgendamentoResponse
        {
            Id = agendamento.Id,
            ClientePessoaId = agendamento.ClientePessoaId,
            ConsultorPessoaId = agendamento.ConsultorPessoaId,
            VeiculoId = agendamento.VeiculoId,
            DataAgendamento = agendamento.DataAgendamento,
            HorarioAgendamento = agendamento.HorarioAgendamento,
            Descricao = agendamento.Descricao,
            CreatedAt = agendamento.CreatedAt,
            UpdatedAt = agendamento.UpdatedAt,
            DeletedAt = agendamento.DeletedAt
        };
    }
}
