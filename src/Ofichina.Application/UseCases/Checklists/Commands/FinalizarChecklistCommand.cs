using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Checklist;

namespace Ofichina.Application.UseCases.Checklists.Commands;

/// <summary>
/// Comando para finalização de checklist.
/// </summary>
public sealed class FinalizarChecklistCommand : ICommand<Result>
{
    public Guid Id { get; init; }

    public FinalizarChecklistCommand(FinalizarChecklistRequest request)
    {
        Id = request.Id;
    }
}