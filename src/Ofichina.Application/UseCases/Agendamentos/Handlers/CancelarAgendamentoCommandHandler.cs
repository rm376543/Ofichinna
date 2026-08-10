using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para cancelar um agendamento existente.
/// </summary>
public sealed class CancelarAgendamentoCommandHandler : ICommandHandler<CancelarAgendamentoCommand>
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelarAgendamentoCommandHandler> _logger;

    public CancelarAgendamentoCommandHandler(
        IAgendamentoRepository agendamentoRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelarAgendamentoCommandHandler> logger)
    {
        _agendamentoRepository = agendamentoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(CancelarAgendamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Cancelando agendamento. AgendamentoId: {AgendamentoId}", command.AgendamentoId);

            var agendamento = await _agendamentoRepository.GetByIdAsync(command.AgendamentoId, cancellationToken, tracking: true);
            if (agendamento is null || agendamento.EstaExcluida())
                return Result.Failure("Agendamento não encontrado.");

            agendamento.Cancelar();

            await _agendamentoRepository.UpdateAsync(agendamento, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Agendamento cancelado com sucesso. AgendamentoId: {AgendamentoId}", agendamento.Id);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao cancelar agendamento. AgendamentoId: {AgendamentoId}", command.AgendamentoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao cancelar agendamento. AgendamentoId: {AgendamentoId}", command.AgendamentoId);
            return Result.Failure("Não foi possível cancelar o agendamento.");
        }
    }
}