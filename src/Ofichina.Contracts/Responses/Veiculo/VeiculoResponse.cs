using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Veiculo;

/// <summary>
/// Resposta resumida de veículo, sem dados da pessoa.
/// </summary>
public sealed class VeiculoResponse : BaseEntity
{
    /// <summary>
    /// Número da placa do veículo.
    /// </summary>
    public string Placa { get; set; } = string.Empty;

    /// <summary>
    /// Marca do veículo.
    /// </summary>
    public string Marca { get; set; } = string.Empty;

    /// <summary>
    /// Modelo do veículo.
    /// </summary>
    public string Modelo { get; set; } = string.Empty;

    /// <summary>
    /// Ano de fabricação do veículo.
    /// </summary>
    public int AnoFabricacao { get; set; }

    /// <summary>
    /// Cor do veículo.
    /// </summary>
    public string Cor { get; set; } = string.Empty;

    /// <summary>
    /// Observações sobre o veículo.
    /// </summary>
    public string? Observacoes { get; set; }

    /// <summary>
    /// Hodômetro do veículo.
    /// </summary>
    public int Hodometro { get; set; }

    /// <summary>
    /// Hodômetro formatado do veículo.
    /// </summary>
    public string HodometroFormatado { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o veículo está ativo.
    /// </summary>
    public bool Ativo { get; set; }
}