using Ofichina.Contracts.Requests.Orcamentos;

namespace Ofichina.Contracts.Requests.Orcamento
{
    /// <summary>
    /// Requisição para atualização de orçamento para um usuário.
    /// </summary>
    public class UpdateOrcamentoRequest : UpdateRequest
    {
        /// <summary>
        /// Cliente relacionado ao orçamento.
        /// </summary>
        public Guid ClienteId { get; init; }

        /// <summary>
        /// Veículo relacionado ao orçamento.
        /// </summary>
        public Guid VeiculoId { get; init; }

        /// <summary>
        /// Responsável pelo orçamento.
        /// </summary>
        public Guid ResponsavelId { get; init; }

        /// <summary>
        /// Data de validade.
        /// </summary>
        public DateTime DataValidade { get; init; }

        /// <summary>
        /// Percentual de desconto geral.
        /// </summary>
        public decimal Desconto { get; init; }

        /// <summary>
        /// Observações.
        /// </summary>
        public string? Observacoes { get; init; }

        /// <summary>
        /// Lista atualizada de serviços.
        /// </summary>
        public ICollection<UpdateOrcamentoServicoRequest> Servicos { get; init; } = [];

        /// <summary>
        /// Lista atualizada de peças.
        /// </summary>
        public ICollection<UpdateOrcamentoPecaRequest> Pecas { get; init; } = [];
    }
}
 