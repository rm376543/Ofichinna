namespace Ofichina.Contracts.Requests.Checklist;

/// <summary>
/// Requisição para finalização de checklist.
/// </summary>
public class FinalizarChecklistRequest
{
    /// <summary>
    /// Identificador do agendamento cujos checklists devem ser finalizados.
    /// </summary>
    public Guid AgendamentoId { get; init; }

}