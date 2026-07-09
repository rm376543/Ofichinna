namespace Ofichina.Contracts.Requests.Veiculos;

/// <summary>
/// Dados necessários para criação de um veículo.
/// </summary>
public sealed class CreateVeiculoRequest : CreateRequest
{
    /// <summary>
    /// Identificador do cliente proprietário.
    /// </summary>
    public Guid ClienteId { get; init; }

    /// <summary>
    /// Placa do veículo.
    /// </summary>
    public string Placa { get; init; } = string.Empty;

    /// <summary>
    /// Marca do veículo.
    /// </summary>
    public string Marca { get; init; } = string.Empty;

    /// <summary>
    /// Modelo do veículo.
    /// </summary>
    public string Modelo { get; init; } = string.Empty;

    /// <summary>
    /// Ano de fabricação.
    /// </summary>
    public int AnoFabricacao { get; init; }

    /// <summary>
    /// Cor do veículo.
    /// </summary>
    public string? Cor { get; init; }

    /// <summary>
    /// Observações sobre o veículo.
    /// </summary>
    public string? Observacoes { get; init; }

    /// <summary>
    /// Indica se o veículo está ativo.
    /// </summary>
    public bool Ativo { get; init; } = true;
}