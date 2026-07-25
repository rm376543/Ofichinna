namespace Ofichina.Contracts.Requests.Veiculo;

/// <summary>
/// Dados necessários para atualização de um veículo.
/// </summary>
public sealed class UpdateVeiculoRequest : UpdateRequest
{
    /// <summary>
    /// Identificador da pessoa proprietária.
    /// </summary>
    public Guid PessoaId { get; init; }

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
    /// Hodometro atual do veículo.
    /// </summary>
    public int Hodometro { get; init; }
}