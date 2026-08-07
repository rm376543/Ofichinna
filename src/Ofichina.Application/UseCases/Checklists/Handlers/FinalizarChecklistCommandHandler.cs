using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Checklists.Handlers;

/// <summary>
/// Handler para finalização de checklist e integração com agendamento.
/// Ao finalizar um agendamento, todos os checklists vinculados são finalizados antes do encerramento do próprio agendamento.
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
#pragma warning disable S6664
            _logger.LogInformation("Finalizando checklists do agendamento. AgendamentoId: {AgendamentoId}", command.AgendamentoId);
#pragma warning restore S6664

            var checklists = (await _checklistRepository.GetAllAsync(cancellationToken))
                .Where(x => x.AgendamentoId == command.AgendamentoId && !x.EstaExcluida())
                .ToList();

            if (checklists.Count == 0)
                return Result.Failure("Nenhum checklist encontrado para o agendamento informado.");

            var agendamento = await _agendamentoRepository.GetByIdAsync(command.AgendamentoId, cancellationToken, tracking: true);
            if (agendamento is null || agendamento.EstaExcluida())
            {
                _logger.LogError("Agendamento não encontrado ou foi excluído. AgendamentoId: {AgendamentoId}", command.AgendamentoId);
                return Result.Failure("Agendamento vinculado não encontrado. A finalização está bloqueada.");
            }

            foreach (var checklist in checklists.Where(x => !x.Finalizado))
            {
                checklist.Finalizar();
                await _checklistRepository.UpdateAsync(checklist, cancellationToken);
            }

            _logger.LogInformation("Checklists finalizados para o agendamento. AgendamentoId: {AgendamentoId}, Quantidade: {Quantidade}", command.AgendamentoId, checklists.Count);

            agendamento.Finalizar();
            await _agendamentoRepository.UpdateAsync(agendamento, cancellationToken);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Agendamento finalizado junto com os checklists. AgendamentoId: {AgendamentoId}", agendamento.Id);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao finalizar checklists. AgendamentoId: {AgendamentoId}", command.AgendamentoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao finalizar checklists. AgendamentoId: {AgendamentoId}", command.AgendamentoId);
            return Result.Failure("Não foi possível finalizar o checklist.");
        }
    }
}