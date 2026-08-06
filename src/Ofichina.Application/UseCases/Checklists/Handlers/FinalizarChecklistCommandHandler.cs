using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Checklists.Handlers;

/// <summary>
/// Handler para finalização de checklist e integração com agendamento.
/// Ao finalizar um checklist vinculado a um agendamento, o agendamento é finalizado automaticamente.
/// Se o checklist estiver vinculado a um agendamento mas o agendamento não existir ou estiver excluído, retorna erro bloqueante.
/// </summary>
public sealed class FinalizarChecklistCommandHandler : ICommandHandler<FinalizarChecklistCommand, Result>
{
    private readonly IRepository<Checklist> _checklistRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FinalizarChecklistCommandHandler> _logger;

    public FinalizarChecklistCommandHandler(
        IRepository<Checklist> checklistRepository,
        IAgendamentoRepository agendamentoRepository,
        IUnitOfWork unitOfWork,
        ILogger<FinalizarChecklistCommandHandler> logger)
    {
        _checklistRepository = checklistRepository;
        _agendamentoRepository = agendamentoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(FinalizarChecklistCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Finalizando checklist. ChecklistId: {ChecklistId}", command.Id);

            var checklist = await _checklistRepository.GetByIdAsync(command.Id, cancellationToken, tracking: true);
            if (checklist is null || checklist.EstaExcluida())
                return Result.Failure("Checklist não encontrado.");

            checklist.Finalizar();

            // Se o checklist estiver vinculado a um agendamento, finalizar também o agendamento
            if (checklist.AgendamentoId.HasValue)
            {
                _logger.LogInformation("Checklist vinculado a agendamento. AgendamentoId: {AgendamentoId}", checklist.AgendamentoId);

                var agendamento = await _agendamentoRepository.GetByIdAsync(checklist.AgendamentoId.Value, cancellationToken, tracking: true);
                if (agendamento is null || agendamento.EstaExcluida())
                {
                    _logger.LogError("Agendamento vinculado ao checklist não encontrado ou foi excluído. ChecklistId: {ChecklistId}, AgendamentoId: {AgendamentoId}",
                        command.Id, checklist.AgendamentoId);
                    return Result.Failure("Agendamento vinculado não encontrado. A finalização está bloqueada.");
                }

                agendamento.Finalizar();
                await _agendamentoRepository.UpdateAsync(agendamento, cancellationToken);

                _logger.LogInformation("Agendamento finalizado junto com o checklist. AgendamentoId: {AgendamentoId}", agendamento.Id);
            }

            await _checklistRepository.UpdateAsync(checklist, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Checklist finalizado com sucesso. ChecklistId: {ChecklistId}", checklist.Id);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao finalizar checklist. ChecklistId: {ChecklistId}", command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao finalizar checklist. ChecklistId: {ChecklistId}", command.Id);
            return Result.Failure("Não foi possível finalizar o checklist.");
        }
    }
}