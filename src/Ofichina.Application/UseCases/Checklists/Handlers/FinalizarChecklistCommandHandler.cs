using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Checklists.Handlers;

/// <summary>
/// Handler para finalização de checklist.
/// </summary>
public sealed class FinalizarChecklistCommandHandler : ICommandHandler<FinalizarChecklistCommand, Result>
{
    private readonly IRepository<Checklist> _checklistRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FinalizarChecklistCommandHandler> _logger;

    public FinalizarChecklistCommandHandler(
        IRepository<Checklist> checklistRepository,
        IUnitOfWork unitOfWork,
        ILogger<FinalizarChecklistCommandHandler> logger)
    {
        _checklistRepository = checklistRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(FinalizarChecklistCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var checklist = await _checklistRepository.GetByIdAsync(command.Id, cancellationToken, tracking: true);
            if (checklist is null || checklist.EstaExcluida())
                return Result.Failure("Checklist não encontrado.");

            checklist.Finalizar();

            await _checklistRepository.UpdateAsync(checklist, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

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