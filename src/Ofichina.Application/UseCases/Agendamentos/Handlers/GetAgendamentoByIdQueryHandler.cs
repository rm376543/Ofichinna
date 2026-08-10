using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Mappings;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para obter um agendamento por Id.
/// </summary>
public sealed class GetAgendamentoByIdQueryHandler : IQueryHandler<GetAgendamentoByIdQuery, Result<AgendamentoUsuarioDetalheResponse>>
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly ILogger<GetAgendamentoByIdQueryHandler> _logger;

    public GetAgendamentoByIdQueryHandler(
        IAgendamentoRepository agendamentoRepository,
        IPessoaRepository pessoaRepository,
        ILogger<GetAgendamentoByIdQueryHandler> logger)
    {
        _agendamentoRepository = agendamentoRepository;
        _pessoaRepository = pessoaRepository;
        _logger = logger;
    }

    public async Task<Result<AgendamentoUsuarioDetalheResponse>> HandleAsync(GetAgendamentoByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(query.PessoaId, cancellationToken);

            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<AgendamentoUsuarioDetalheResponse>("Pessoa não encontrada.");

            var agendamento = await _agendamentoRepository.GetAgendamentoUsuarioViewByIdAsync(query.PessoaId, query.Id, cancellationToken);

            if (agendamento is null)
                return Result.Failure<AgendamentoUsuarioDetalheResponse>("Agendamento não encontrado.");

            return Result.Success(agendamento.ToDetalheResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter agendamento por Id. AgendamentoId: {AgendamentoId}", query.Id);
            return Result.Failure<AgendamentoUsuarioDetalheResponse>("Não foi possível obter o agendamento.");
        }
    }
}


