using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para obter um agendamento por Id.
/// </summary>
public sealed class GetAgendamentoByIdQueryHandler : IQueryHandler<GetAgendamentoByIdQuery, Result<AgendamentoResponse>>
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

    public async Task<Result<AgendamentoResponse>> HandleAsync(GetAgendamentoByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(query.PessoaId, cancellationToken);

            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Pessoa não encontrada.");

            var agendamento = await _agendamentoRepository.GetByIdAndPessoaAsync(query.Id, query.PessoaId, cancellationToken);

            if (agendamento is null)
                return Result.Failure<AgendamentoResponse>("Agendamento não encontrado.");

            return Result.Success(Mapear(agendamento));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter agendamento por Id. AgendamentoId: {AgendamentoId}", query.Id);
            return Result.Failure<AgendamentoResponse>("Não foi possível obter o agendamento.");
        }
    }

    private static AgendamentoResponse Mapear(Agendamento agendamento)
    {
        return new AgendamentoResponse
        {
            Id = agendamento.Id,
            ClientePessoaId = agendamento.ClientePessoaId,
            ClienteNome = agendamento.Cliente.Nome,
            DiaDisponibilidadeId = agendamento.DiaDisponibilidadeId,
            HorarioConsultorId = agendamento.HorarioConsultorId,
            ConsultorPessoaId = agendamento.ConsultorPessoaId,
            ConsultorNome = agendamento.HorarioConsultor.Pessoa.Nome,
            VeiculoId = agendamento.VeiculoId,
            VeiculoPlaca = agendamento.Veiculo.Placa.Numero,
            VeiculoDescricao = $"{agendamento.Veiculo.Marca} {agendamento.Veiculo.Modelo} {agendamento.Veiculo.AnoFabricacao}",
            Status = agendamento.Status.ToString(),
            Descricao = agendamento.Descricao,
            CreatedAt = agendamento.CreatedAt,
            UpdatedAt = agendamento.UpdatedAt,
            DeletedAt = agendamento.DeletedAt
        };
    }
}


