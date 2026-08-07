using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.OrdensServico;

/// <summary>
/// Resposta com os dados de uma peça vinculada a um serviço da ordem de serviço.
/// </summary>
public sealed class OrdemServicoPecaResponse : BaseResponse
{
    /// <summary>
    /// Identificador do vínculo resposta.
    /// </summary>
    public Guid OrdemServicoPecaId { get; set; }

    /// <summary>
    /// Identificador da peça cadastrada.
    /// </summary>
    public Guid PecaId { get; set; }

    /// <summary>
    /// Identificador do item de serviço no contexto da ordem.
    /// </summary>
    public Guid ItemServicoId { get; set; }

    /// <summary>
    /// Identificador do serviço cadastrado ao qual a peça pertence.
    /// </summary>
    public Guid ServicoId { get; set; }

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
}
