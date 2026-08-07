using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Authentication;
using Ofichina.Application.Extensions;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para criação de agendamento usando o novo modelo com HorarioConsultorDisponibilidade.
/// </summary>
public sealed class CreateAgendamentoCommandHandler : ICommandHandler<CreateAgendamentoCommand, Result<AgendamentoResponse>>
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IHorarioConsultorDisponibilidadeRepository _horarioConsultorDisponibilidadeRepository;
    private readonly IPerfilAutorizacaoService _perfilAutorizacaoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAgendamentoCommandHandler> _logger;

    public CreateAgendamentoCommandHandler(
        IAgendamentoRepository agendamentoRepository,
        IPessoaRepository pessoaRepository,
        IVeiculoRepository veiculoRepository,
        IHorarioConsultorDisponibilidadeRepository horarioConsultorDisponibilidadeRepository,
        IPerfilAutorizacaoService perfilAutorizacaoService,
        IUnitOfWork unitOfWork,
        ILogger<CreateAgendamentoCommandHandler> logger)
    {
        _agendamentoRepository = agendamentoRepository;
        _pessoaRepository = pessoaRepository;
        _veiculoRepository = veiculoRepository;
        _horarioConsultorDisponibilidadeRepository = horarioConsultorDisponibilidadeRepository;
        _perfilAutorizacaoService = perfilAutorizacaoService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AgendamentoResponse>> HandleAsync(CreateAgendamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de agendamento. PessoaId: {PessoaId}, SlotId: {SlotId}",
                command.PessoaId, command.HorarioConsultorDisponibilidadeId);

            // Validar pessoa cliente
            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);
            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Pessoa não encontrada.");

            // Validar veículo
            var veiculo = await _veiculoRepository.GetByIdAsync(command.VeiculoId, includePessoa: true, cancellationToken);
            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Veículo não encontrado.");

            if (veiculo.PessoaId != pessoa.Id)
                return Result.Failure<AgendamentoResponse>("O veículo informado não pertence ao usuário autenticado.");

            // Validar slot de disponibilidade (novo modelo)
            var slot = await _horarioConsultorDisponibilidadeRepository.GetByIdAsync(
                command.HorarioConsultorDisponibilidadeId, cancellationToken);

            if (slot is null || slot.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Slot de disponibilidade não encontrado.");

            // Validar consultor
            var consultor = slot.Consultor;
            if (consultor is null || consultor.EstaExcluida())
                return Result.Failure<AgendamentoResponse>("Consultor não encontrado.");

            var possuiPerfilConsultor = await _perfilAutorizacaoService.PossuiPerfilAsync(consultor.UsuarioId, "CONSULTOR", cancellationToken);
            if (!possuiPerfilConsultor)
                return Result.Failure<AgendamentoResponse>("A pessoa informada não possui perfil de consultor.");

            // Validar conflito: verificar se já existe agendamento neste slot
            var agendamentosSlot = await _agendamentoRepository.GetAllAsync(cancellationToken);
            var jaTemAgendamentoNoSlot = agendamentosSlot
                .Any(a => a.HorarioConsultorDisponibilidadeId == command.HorarioConsultorDisponibilidadeId && a.DeletedAt == null);

            if (jaTemAgendamentoNoSlot)
                return Result.Failure<AgendamentoResponse>("Já existe um agendamento para este horário/consultor.");

            // Validar conflito: verificar se veículo já tem agendamento no mesmo dia/horário
            var veiculoJaAgendado = agendamentosSlot
                .Where(a => a.VeiculoId == command.VeiculoId && a.DeletedAt == null)
                .Any(a => a.HorarioConsultorDisponibilidade.DiaDisponibilidadeId == slot.DiaDisponibilidadeId &&
                          a.HorarioConsultorDisponibilidade.HorarioDisponibilidadeId == slot.HorarioDisponibilidadeId);

            if (veiculoJaAgendado)
                return Result.Failure<AgendamentoResponse>("Já existe um agendamento para este veículo neste horário.");

            // Criar novo agendamento usando o construtor do novo modelo
            var agendamento = new Agendamento(
                command.PessoaId,
                command.HorarioConsultorDisponibilidadeId,
                command.VeiculoId,
                command.Descricao);

            await _agendamentoRepository.AddAsync(agendamento, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Agendamento criado com sucesso. AgendamentoId: {AgendamentoId}", agendamento.Id);

            return Result.Success(agendamento.ToResponse());
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
}
