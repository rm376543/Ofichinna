namespace Ofichina.Application.UseCases.Checklists.Commands;

using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Checklist;

/// <summary>
/// Descrição do comando.
/// </summary>
public sealed class RemoveChecklistCommand : ICommand<Result>
{
    public Guid Id { get; init; }

    public RemoveChecklistCommand(RemoveChecklistRequest request)
    {
        Id = request.ChecklistId;
    }
}


