namespace Ofichina.Contracts.Requests.Checklist;

/// <summary>
/// Requisição para finalização de checklist.
/// </summary>
public sealed class FinalizarChecklistRequest
{
    /// <summary>
    /// Identificador do checklist a ser finalizado.
    /// </summary>
    public Guid Id { get; }

    public FinalizarChecklistRequest(Guid checklistId)
    {
        Id = checklistId;
    }
}