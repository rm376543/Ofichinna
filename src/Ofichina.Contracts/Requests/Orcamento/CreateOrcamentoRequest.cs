using Ofichina.Contracts.Requests.Orcamentos;

namespace Ofichina.Contracts.Requests.Cliente;

/// <summary>
/// Requisição para cadastro de novo orçamento para um usuário.
/// </summary>
public sealed class CreateOrcamentoRequest : CreateRequest
{
    /// <summary>
    /// Cliente do orçamento.
    /// </summary>
    public Guid ClienteId { get; init; }

    /// <summary>
    /// Veículo do orçamento.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Funcionário responsável.
    /// </summary>
    public Guid ResponsavelId { get; init; }

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

    /// <summary>
    /// Serviços.
    /// </summary>
    public ICollection<CreateOrcamentoServicoRequest> Servicos { get; init; } = [];

    /// <summary>
    /// Peças.
    /// </summary>
    public ICollection<CreateOrcamentoPecaRequest> Pecas { get; init; } = [];
}