using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Checklists.Handlers;

/// <summary>
/// Handler para criação de checklist.
/// </summary>
public sealed class CreateChecklistCommandHandler : ICommandHandler<CreateChecklistCommand, Result>
{
    private readonly IRepository<Checklist> _checklistRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateChecklistCommandHandler> _logger;

    public CreateChecklistCommandHandler(
        IRepository<Checklist> checklistRepository,
        IAgendamentoRepository agendamentoRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateChecklistCommandHandler> logger)
    {
        _checklistRepository = checklistRepository;
        _agendamentoRepository = agendamentoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(CreateChecklistCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var agendamento = await _agendamentoRepository.GetByIdAsync(command.AgendamentoId, cancellationToken, tracking: true);
            if (agendamento is null || agendamento.EstaExcluida())
                return Result.Failure("Agendamento não encontrado.");

            var checklist = agendamento.CriarChecklist(
                command.ItensVerificados,
                command.Observacoes);

            await _checklistRepository.AddAsync(checklist, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar checklist.");
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar checklist.");
            return Result.Failure("Não foi possível criar o checklist.");
        }
    }
}