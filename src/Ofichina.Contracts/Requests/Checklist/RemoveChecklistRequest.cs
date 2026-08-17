namespace Ofichina.Contracts.Requests.Checklist
{
    /// <summary>
    /// Representa a solicitação para remover um checklist existente.
    /// </summary>
    public class RemoveChecklistRequest
    {
        public Guid AgendamentoId { get; set; }
        /// <summary>
        /// Id do checklist a ser removido.
        /// </summary>
        public Guid ChecklistId { get; set; }

    }
}
