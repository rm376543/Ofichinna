using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Authentication.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para criação de agendamento.
/// </summary>
public sealed class CreateAgendamentoCommandHandler : ICommandHandler<CreateAgendamentoCommand, Result<AgendamentoResponse>>
{
    private readonly IRepository<Agendamento> _agendamentoRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAgendamentoCommandHandler> _logger;

    public CreateAgendamentoCommandHandler(
        IRepository<Agendamento> agendamentoRepository,
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

    public async Task<Result<AgendamentoResponse>> HandleAsync(CreateAgendamentoCommand command)
    {
        try
        {
            var usuarioId = _usuarioAtualService.ObterUsuarioId();

            if (usuarioId is null)
                return Result.Failure<AgendamentoResponse>("Usuário autenticado não encontrado.");

            _logger.LogInformation("Iniciando a criação de agendamento para o usuário {UsuarioId}.", usuarioId);

            var pessoa = await _pessoaRepository.GetByUsuarioIdAsync(usuarioId.Value);
            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Pessoa não encontrada para o usuário autenticado.");

            var veiculo = await _veiculoRepository.GetByIdWithPessoaAsync(command.VeiculoId);
            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Veículo não encontrado.");

            if (veiculo.PessoaId != pessoa.Id)
                return Result.Failure<AgendamentoResponse>("O veículo informado não pertence ao usuário autenticado.");

            var dataHoraAgendada = command.DataHoraPreferida.ToUniversalTime();

            if (dataHoraAgendada <= DateTime.UtcNow)
                return Result.Failure<AgendamentoResponse>("A data e hora do agendamento devem ser futuras.");

            var agendamentoExistente = (await _agendamentoRepository.GetAllAsync())
                .FirstOrDefault(x =>
                    !x.EstaExcluida() &&
                    x.VeiculoId == command.VeiculoId &&
                    x.DataHoraAgendada == dataHoraAgendada &&
                    x.Status != StatusAgendamento.Cancelado);

            if (agendamentoExistente is not null)
                return Result.Failure<AgendamentoResponse>("Já existe um agendamento para este horário.");

            var agendamento = new Agendamento(
                pessoa.Id,
                command.VeiculoId,
                dataHoraAgendada,
                command.Motivo,
                command.Observacoes,
                CanalAtendimento.Aplicativo);

            await _agendamentoRepository.AddAsync(agendamento);
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
            PessoaId = agendamento.PessoaId,
            VeiculoId = agendamento.VeiculoId,
            DataHoraAgendada = agendamento.DataHoraAgendada,
            Motivo = agendamento.Motivo,
            Observacoes = agendamento.Observacoes,
            Status = agendamento.Status.ToString(),
            CanalAtendimento = agendamento.CanalAtendimento.ToString(),
            CreatedAt = agendamento.CreatedAt,
            UpdatedAt = agendamento.UpdatedAt,
            DeletedAt = agendamento.DeletedAt
        };
    }
}