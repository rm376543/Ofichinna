namespace Ofichina.Contracts.Responses.Veiculo;

/// <summary>
/// Resumo da pessoa proprietária do veículo.
/// </summary>
public sealed class VeiculoPessoaResponse
{
    /// <summary>
    /// Nome da pessoa.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Documento da pessoa.
    /// </summary>
    public string Documento { get; set; } = string.Empty;

    /// <summary>
    /// Telefone da pessoa.
    /// </summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// Indica se a pessoa está ativa.
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data de criação da pessoa.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização da pessoa.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Data da exclusão lógica da pessoa.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}