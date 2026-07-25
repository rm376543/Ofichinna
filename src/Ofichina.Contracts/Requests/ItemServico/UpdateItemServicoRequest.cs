namespace Ofichina.Contracts.Requests.ItensServico;

/// <summary>
/// Dados necessários para atualização de um item de serviço na ordem de serviço.
/// </summary>
public sealed class UpdateItemServicoRequest : UpdateRequest
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; } = Guid.Empty;

    /// <summary>
    /// Identificador dos Servicos e Pecas vinculados a uma Ordem de Servico.
    /// </summary>
    public Guid ServicoPecaId { get; init; } = Guid.Empty;
}

