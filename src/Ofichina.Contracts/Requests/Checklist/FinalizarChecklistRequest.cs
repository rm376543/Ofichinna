namespace Ofichina.Contracts.Requests.Checklist;

/// <summary>
/// Requisição para finalização de checklist.
/// </summary>
public class FinalizarChecklistRequest
{
    /// <summary>
    /// Identificador do checklist a ser finalizado.
    /// </summary>
    public Guid Id { get; init; }

}