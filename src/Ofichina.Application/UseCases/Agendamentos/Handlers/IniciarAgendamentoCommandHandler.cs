using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para iniciar um agendamento existente.
/// </summary>
public sealed class IniciarAgendamentoCommandHandler : ICommandHandler<IniciarAgendamentoCommand>
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IniciarAgendamentoCommandHandler> _logger;

    public IniciarAgendamentoCommandHandler(
        IAgendamentoRepository agendamentoRepository,
        IUnitOfWork unitOfWork,
        ILogger<IniciarAgendamentoCommandHandler> logger)
    {
        _agendamentoRepository = agendamentoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(IniciarAgendamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando agendamento. AgendamentoId: {AgendamentoId}", command.AgendamentoId);

            var agendamento = await _agendamentoRepository.GetByIdAsync(command.AgendamentoId, cancellationToken, tracking: true);
            if (agendamento is null || agendamento.EstaExcluida())
                return Result.Failure("Agendamento não encontrado.");

            agendamento.Iniciar();

            await _agendamentoRepository.UpdateAsync(agendamento, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Agendamento iniciado com sucesso. AgendamentoId: {AgendamentoId}", agendamento.Id);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao iniciar agendamento. AgendamentoId: {AgendamentoId}", command.AgendamentoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao iniciar agendamento. AgendamentoId: {AgendamentoId}", command.AgendamentoId);
            return Result.Failure("Não foi possível iniciar o agendamento.");
        }
    }
}
