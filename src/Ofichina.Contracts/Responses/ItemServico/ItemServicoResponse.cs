using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdensServico;

namespace Ofichina.Contracts.Responses.ItensServico;

/// <summary>
/// Resposta com os dados de um item de serviço vinculado à ordem de serviço.
/// </summary>
public sealed class ItemServicoResponse : BaseEntity
{
    /// <summary>
    /// Identificador do serviço cadastrado vinculado ao item.
    /// </summary>
    public Guid ServicoId { get; set; }

    /// <summary>
    /// Identificador da ordem de serviço à qual o item pertence.
    /// </summary>
    public Guid OrdemServicoId { get; set; }

    /// <summary>
    /// Descrição do serviço.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Valor do serviço.
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Valor total do item de serviço.
    /// </summary>
    public decimal ValorTotal { get; set; }

    /// <summary>
    /// Peças vinculadas ao item de serviço.
    /// </summary>
    public ICollection<OrdemServicoPecaResponse> Pecas { get; set; } = [];
}
