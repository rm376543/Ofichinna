namespace Ofichina.Contracts.Requests.Orcamento
{
    /// <summary>
    /// Requisição para atualização de orçamento para um usuário.
    /// </summary>
    public class UpdateOrcamentoRequest : UpdateRequest
    {
        /// <summary>
        /// Pessoa relacionada ao orçamento.
        /// </summary>
        public Guid PessoaId { get; init; }

        /// <summary>
        /// Veículo relacionado ao orçamento.
        /// </summary>
        public Guid VeiculoId { get; init; }

        /// <summary>
        /// Responsável pelo orçamento.
        /// </summary>
        public Guid ResponsavelId { get; init; }

        /// <summary>
        /// Mecânico responsável pelo diagnóstico.
        /// </summary>
        public Guid MecanicoDiagnosticoId { get; init; }

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
        /// Lista atualizada de itens de serviço.
        /// </summary>
        public ICollection<OrcamentoItemServicoRequest> ItensServico { get; init; } = [];
    }
}
