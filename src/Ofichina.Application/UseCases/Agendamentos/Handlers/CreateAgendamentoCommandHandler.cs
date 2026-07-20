using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;
using Ofichina.Authentication.Abstractions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para criação de agendamento.
/// </summary>
public sealed class CreateAgendamentoCommandHandler : ICommandHandler<CreateAgendamentoCommand, Result<AgendamentoResponse>>
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAgendamentoCommandHandler> _logger;

    public CreateAgendamentoCommandHandler(
        IAgendamentoRepository agendamentoRepository,
        IPessoaRepository pessoaRepository,
        IVeiculoRepository veiculoRepository,
        IUsuarioAtualService usuarioAtualService,
        IUnitOfWork unitOfWork,
        ILogger<CreateAgendamentoCommandHandler> logger)
    {
        _agendamentoRepository = agendamentoRepository;
        _pessoaRepository = pessoaRepository;
        _veiculoRepository = veiculoRepository;
        _usuarioAtualService = usuarioAtualService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AgendamentoResponse>> HandleAsync(CreateAgendamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a criação de agendamento para a pessoa {PessoaId}.", command.PessoaId);

            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);
            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Pessoa não encontrada.");

            var veiculo = await _veiculoRepository.GetByIdWithPessoaAsync(command.VeiculoId, cancellationToken);
            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Veículo não encontrado.");

            if (veiculo.PessoaId != pessoa.Id)
                return Result.Failure<AgendamentoResponse>("O veículo informado não pertence ao usuário autenticado.");

            var consultor = await _pessoaRepository.GetByIdAsync(command.ConsultorPessoaId, cancellationToken);
            if (consultor is null || consultor.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Consultor não encontrado.");

            var agendamentoExistente = (await _agendamentoRepository.GetAllAsync(cancellationToken))
                .FirstOrDefault(x =>
                    !x.EstaExcluida() &&
                    x.ConsultorPessoaId == command.ConsultorPessoaId &&
                    x.DataAgendamento == command.DataAgendamento &&
                    x.HorarioAgendamento == command.HorarioAgendamento);

            if (agendamentoExistente is not null)
                return Result.Failure<AgendamentoResponse>("Já existe um agendamento para este horário.");

            var veiculoConflito = (await _agendamentoRepository.GetAllAsync(cancellationToken))
                .FirstOrDefault(x =>
                    !x.EstaExcluida() &&
                    x.VeiculoId == command.VeiculoId &&
                    x.DataAgendamento == command.DataAgendamento &&
                    x.HorarioAgendamento == command.HorarioAgendamento);

            if (veiculoConflito is not null)
                return Result.Failure<AgendamentoResponse>("Já existe um agendamento para este veículo neste horário.");

            var agendamento = new Agendamento(
                command.PessoaId,
                command.ConsultorPessoaId,
                command.VeiculoId,
                command.DataAgendamento,
                command.HorarioAgendamento,
                command.Descricao);

            await _agendamentoRepository.AddAsync(agendamento, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Agendamento criado com sucesso. AgendamentoId: {AgendamentoId}", agendamento.Id);

            return Result.Success(Mapear(agendamento));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar agendamento.");
            return Result.Failure<AgendamentoResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar agendamento.");
            return Result.Failure<AgendamentoResponse>("Não foi possível criar o agendamento.");
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


