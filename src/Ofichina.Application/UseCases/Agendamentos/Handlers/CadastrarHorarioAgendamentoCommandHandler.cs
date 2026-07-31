using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers
{
    public sealed class CadastrarHorarioAgendamentoCommandHandler : ICommandHandler<CadastraHorarioAgendamentoCommand, Result>
    {
        private readonly IHorarioDisponibilidadeRepository _horarioDisponibilidadeRepository;
        private readonly ILogger<CadastrarHorarioAgendamentoCommandHandler> _logger;
        private readonly IUnitOfWork unitOfWork;

        public CadastrarHorarioAgendamentoCommandHandler(
            IHorarioDisponibilidadeRepository horarioDisponibilidadeRepository,
            ILogger<CadastrarHorarioAgendamentoCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _horarioDisponibilidadeRepository = horarioDisponibilidadeRepository;
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result> HandleAsync(CadastraHorarioAgendamentoCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Iniciando o cadastro do horário de agendamento");

                var buscarHorarioExistente = await _horarioDisponibilidadeRepository.BuscarPorHorarioAsync(command.Horario, cancellationToken);

                if (buscarHorarioExistente != null)
                {
                    _logger.LogWarning("O horário de agendamento já existe: {Horario}", command.Horario);
                    return Result.Failure("O horário de agendamento já existe.");
                }

                var horario = new HorarioDisponibilidade(command.Horario);

                await _horarioDisponibilidadeRepository.AddAsync(horario);
                await unitOfWork.SaveChangesAsync();

                return Result.Success();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar horário de agendamento.");
                return Result.Failure("Ocorreu um erro ao cadastrar o horário de agendamento.");
            }
        }
    }
}
