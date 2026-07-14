namespace Ofichina.Contracts.Requests.OrdemServicos;

/// <summary>
/// Dados necessários para atualização de uma ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoRequest : UpdateRequest
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Funcionário responsável pelo atendimento.
    /// </summary>
    public Guid FuncionarioId { get; init; }

    /// <summary>
    /// Descrição atualizada do problema relatado pelo cliente.
    /// </summary>
    public string ProblemaRelatado { get; init; } = string.Empty;

    /// <summary>
    /// Observações da ordem de serviço.
    /// </summary>
    public string? Observacoes { get; init; }

    /// <summary>
    /// Serviços previstos na ordem.
    /// </summary>
    public ICollection<UpdateOrdemServicoItemServicoRequest> Servicos { get; init; }
        = [];

    /// <summary>
    /// Peças previstas na ordem.
    /// </summary>
    public ICollection<UpdateOrdemServicoItemPecaRequest> Pecas { get; init; }
        = [];
}