using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.Orcamento
{
    /// <summary>
    /// Requisição para atualização do cabeçalho de orçamento para um usuário.
    /// </summary>
    public class UpdateOrcamentoRequest : UpdateRequest
    {
        /// <summary>
        /// Identificador do orçamento.
        /// </summary>
        public Guid OrcamentoId { get; init; }

        /// <summary>
        /// Pessoa relacionada ao orçamento.
        /// </summary>
        public Guid PessoaId { get; init; }

        /// <summary>
        /// Veículo relacionado ao orçamento.
        /// </summary>
        public Guid VeiculoId { get; init; }

        /// <summary>
        /// Consultor do orçamento.
        /// </summary>
        public Guid ConsultorId { get; init; }

        /// <summary>
        /// Mecânico responsável pelo diagnóstico.
        /// </summary>
        public Guid MecanicoId { get; init; }

        /// <summary>
        /// Data de validade.
        /// </summary>
        public DateOnly DataValidade { get; init; }

        /// <summary>
        /// Observações.
        /// </summary>
        public string? Observacoes { get; init; }
    }
}
