using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.OrdemServico;

/// <summary>
/// Dados necessários para atualização de uma ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoRequest : UpdateRequest
{
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
}