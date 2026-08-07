using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamentos;

namespace Ofichina.Contracts.Requests.Orcamento;

/// <summary>
/// Requisição para cadastro de novo orçamento para um usuário.
/// </summary>
public sealed class CreateOrcamentoRequest : CreateRequest
{
    /// <summary>
    /// Pessoa do orçamento.
    /// </summary>
    public Guid PessoaId { get; init; }

    /// <summary>
    /// Veículo do orçamento.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Agendamento de origem do orçamento.
    /// </summary>
    public Guid AgendamentoId { get; init; }

    /// <summary>
    /// Responsável.
    /// </summary>
    public Guid ResponsavelId { get; init; }

    /// <summary>
    /// Mecânico responsável pelo diagnóstico.
    /// </summary>
    public Guid MecanicoDiagnosticoId { get; init; }

    /// <summary>
    /// Data de validade do orçamento.
    /// </summary>
    public DateTime DataValidade { get; init; }

    /// <summary>
    /// Observações.
    /// </summary>
    public string? Observacoes { get; init; }

    /// <summary>
    /// Percentual de desconto.
    /// </summary>
    public decimal Desconto { get; init; }

}