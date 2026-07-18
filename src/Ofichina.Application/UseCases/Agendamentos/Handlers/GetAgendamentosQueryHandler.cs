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
/// Handler para listar agendamentos do usuÃ¡rio autenticado.
/// </summary>
public sealed class GetAgendamentosQueryHandler : IQueryHandler<GetAgendamentosQuery, Result<IReadOnlyCollection<AgendamentoResponse>>>
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly ILogger<GetAgendamentosQueryHandler> _logger;

    public GetAgendamentosQueryHandler(
        IAgendamentoRepository agendamentoRepository,
        IPessoaRepository pessoaRepository,
        IUsuarioAtualService usuarioAtualService,
        ILogger<GetAgendamentosQueryHandler> logger)
    {
        _agendamentoRepository = agendamentoRepository;
        _pessoaRepository = pessoaRepository;
        _usuarioAtualService = usuarioAtualService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<AgendamentoResponse>>> HandleAsync(GetAgendamentosQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(query.PessoaId, cancellationToken);

            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<IReadOnlyCollection<AgendamentoResponse>>("Pessoa nÃ£o encontrada.");

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
            return Result.Failure<IReadOnlyCollection<AgendamentoResponse>>("NÃ£o foi possÃ­vel obter os agendamentos.");
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
