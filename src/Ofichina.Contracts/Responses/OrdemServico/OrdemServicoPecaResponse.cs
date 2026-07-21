namespace Ofichina.Contracts.Responses.OrdemServico;

/// <summary>
/// Resposta com os dados de uma peça vinculada a um serviço da ordem de serviço.
/// </summary>
public sealed class OrdemServicoPecaResponse
{
    /// <summary>
    /// Identificador da peça vinculada ao serviço.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador da peça cadastrada.
    /// </summary>
    public Guid PecaId { get; set; }

    /// <summary>
    /// Identificador do item de serviço ao qual a peça pertence.
    /// </summary>
    public Guid ItemServicoId { get; set; }

    /// <summary>
    /// Descrição da peça.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade utilizada ou prevista.
    /// </summary>
    public int Quantidade { get; set; }

    /// <summary>
    /// Valor unitário da peça.
    /// </summary>
    public decimal ValorUnitario { get; set; }

    /// <summary>
    /// Valor total da peça.
    /// </summary>
    public decimal ValorTotal { get; set; }

    /// <summary>
    /// Indica se a peça já foi utilizada.
    /// </summary>
    public bool Utilizada { get; set; }

    /// <summary>
    /// Data em que a peça foi utilizada.
    /// </summary>
    public DateTime? DataUtilizacao { get; set; }

    /// <summary>
    /// Data de criação do registro.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização do registro.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Data da exclusão lógica do registro.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
