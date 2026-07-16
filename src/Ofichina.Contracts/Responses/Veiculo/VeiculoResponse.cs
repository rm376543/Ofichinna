namespace Ofichina.Contracts.Responses.Veiculo;

/// <summary>
/// Resposta com os dados completos de um veículo.
/// </summary>
public sealed class VeiculoResponse
{
    /// <summary>
    /// Identificador do veículo.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Dados resumidos da pessoa proprietária.
    /// </summary>
    public VeiculoPessoaResponse Pessoa { get; set; } = new();

    /// <summary>
    /// Placa do veículo.
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
    /// Ano de fabricação.
    /// </summary>
    public int AnoFabricacao { get; set; }

    /// <summary>
    /// Cor do veículo.
    /// </summary>
    public string Cor { get; set; } = string.Empty;

    /// <summary>
    /// Observações adicionais.
    /// </summary>
    public string? Observacoes { get; set; }

    /// <summary>
    /// Hodometro atual.
    /// </summary>
    public int Hodometro { get; set; }

    /// <summary>
    /// Hodometro atual formatada para leitura.
    /// </summary>
    public string HodometroFormatada { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o veículo está ativo.
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data de criação do registro.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Data da exclusão lógica.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}