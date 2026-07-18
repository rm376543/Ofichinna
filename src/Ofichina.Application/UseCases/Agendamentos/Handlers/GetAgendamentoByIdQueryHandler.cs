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
/// Handler para obter um agendamento por Id.
/// </summary>
public sealed class GetAgendamentoByIdQueryHandler : IQueryHandler<GetAgendamentoByIdQuery, Result<AgendamentoResponse>>
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly ILogger<GetAgendamentoByIdQueryHandler> _logger;

    public GetAgendamentoByIdQueryHandler(
        IAgendamentoRepository agendamentoRepository,
        IPessoaRepository pessoaRepository,
        IUsuarioAtualService usuarioAtualService,
        ILogger<GetAgendamentoByIdQueryHandler> logger)
    {
        _agendamentoRepository = agendamentoRepository;
        _pessoaRepository = pessoaRepository;
        _usuarioAtualService = usuarioAtualService;
        _logger = logger;
    }

    public async Task<Result<AgendamentoResponse>> HandleAsync(GetAgendamentoByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(query.PessoaId, cancellationToken);

            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Pessoa nÃ£o encontrada.");

            var agendamento = (await _agendamentoRepository.GetAllAsync(cancellationToken))
                .FirstOrDefault(x => !x.EstaExcluida() && x.Id == query.Id && x.ClientePessoaId == query.PessoaId);

            if (agendamento is null)
                return Result.Failure<AgendamentoResponse>("Agendamento nÃ£o encontrado.");

            return Result.Success(Mapear(agendamento));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter agendamento por Id. AgendamentoId: {AgendamentoId}", query.Id);
            return Result.Failure<AgendamentoResponse>("NÃ£o foi possÃ­vel obter o agendamento.");
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
