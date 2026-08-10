using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Mappings;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para listar agendamentos do usuário autenticado.
/// </summary>
public sealed class GetAgendamentosQueryHandler : IQueryHandler<GetAgendamentosQuery, Result<IReadOnlyCollection<AgendamentoUsuarioResponse>>>
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

    public async Task<Result<IReadOnlyCollection<AgendamentoUsuarioResponse>>> HandleAsync(GetAgendamentosQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(query.PessoaId, cancellationToken);

            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<IReadOnlyCollection<AgendamentoUsuarioResponse>>("Pessoa não encontrada.");

            var agendamentosView = await _agendamentoRepository.GetAgendamentosUsuarioViewByPessoaAsync(query.PessoaId, cancellationToken);

            var resultado = agendamentosView
                .Select(agendamento => agendamento.ToUsuarioResponse())
                .ToList();

            return Result.Success<IReadOnlyCollection<AgendamentoUsuarioResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar agendamentos.");
            return Result.Failure<IReadOnlyCollection<AgendamentoUsuarioResponse>>("Não foi possível obter os agendamentos.");
        }
    }
}
