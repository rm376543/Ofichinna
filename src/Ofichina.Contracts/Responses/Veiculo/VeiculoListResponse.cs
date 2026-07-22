namespace Ofichina.Contracts.Responses.Veiculo;

/// <summary>
/// Resposta resumida de veículo, sem dados da pessoa.
/// </summary>
public sealed class VeiculoListResponse
{
    public Guid Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int AnoFabricacao { get; set; }
    public string Cor { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public int Hodometro { get; set; }
    public string HodometroFormatada { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}