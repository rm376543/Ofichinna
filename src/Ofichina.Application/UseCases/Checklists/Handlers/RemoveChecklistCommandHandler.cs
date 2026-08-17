namespace Ofichina.Application.UseCases.Checklists.Handlers;

using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Contracts.Common;

public sealed class RemoveChecklistCommandHandler : ICommandHandler<RemoveChecklistCommand, Result>
{
    private readonly IChecklistRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveChecklistCommandHandler> _logger;

    public RemoveChecklistCommandHandler(
        IChecklistRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveChecklistCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(
        RemoveChecklistCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando processamento do comando.");

            var checklist = await _repository.GetByAgendamentoChecklistIdAsync(command.AgendamentoId, command.ChecklistId, cancellationToken);
            if (checklist == null)
            {
                _logger.LogWarning("Checklist não encontrado.");
                return Result.Failure("Checklist não encontrado.");
            }

            if (checklist.EstaExcluida())
            {
                _logger.LogWarning("Checklist já está excluída.");
                return Result.Failure("Checklist já está excluída.");
            }

            if (checklist.EstaFinalizado())
            {
                _logger.LogWarning("Não é possível remover o checklist porque ele já está finalizado.");
                return Result.Failure("Checklist já está finalizado.");
            }

            checklist.Excluir();

            await _repository.UpdateAsync(checklist, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar comando.");
            return Result.Failure("Ocorreu um erro.");
        }
    }
}


