namespace Ofichina.Contracts.Requests.ItemServico
{
    /// <summary>
    /// Dados necessários para exclusão de um item de serviço na ordem de serviço.
    /// </summary>
    public class DeleteItemServicoRequest
    {
        /// <summary>
        /// Identificador da ordem de serviço.
        /// </summary>
        public Guid OrdemServicoId { get; set; }

        /// <summary>
        /// Identificador do item de serviço.
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty;
    }
}
