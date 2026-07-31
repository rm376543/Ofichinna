using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers
{
    /// <summary>
    /// Handler para o comando de cancelamento de agendamento.
    /// </summary>
    public sealed class CancelarAgendamentoCommandHandler : ICommandHandler<CancelarAgendamentoCommand, Result>
    {
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly ILogger<CancelarAgendamentoCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CancelarAgendamentoCommandHandler(
            IAgendamentoRepository agendamentoRepository,
            ILogger<CancelarAgendamentoCommandHandler> logger,
            IUnitOfWork unitOfWork
            )
        {
            _agendamentoRepository = agendamentoRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> HandleAsync(CancelarAgendamentoCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Iniciando o cancelamento do agendamento com ID: {AgendamentoId} para a pessoa com ID: {PessoaId}", command.AgendamentoId, command.PessoaId);

                var agendamento = await _agendamentoRepository.BuscarAgendamentosPorPessoaId(command.PessoaId);

                if (agendamento == null)
                {
                    _logger.LogWarning("Agendamento não encontrado para a pessoa com ID: {PessoaId}", command.PessoaId);
                    return Result.Failure($"Agendamento não encontrado para a pessoa com ID: {command.PessoaId}");
                }

                agendamento.Cancelar();
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Agendamento com ID: {AgendamentoId} para a pessoa com ID: {PessoaId} cancelado com sucesso.", command.AgendamentoId, command.PessoaId);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar o agendamento com ID: {AgendamentoId} para a pessoa com ID: {PessoaId}", command.AgendamentoId, command.PessoaId);
                return Result.Failure($"Erro ao cancelar o agendamento: {ex.Message}");
            }
        }
    }
}
