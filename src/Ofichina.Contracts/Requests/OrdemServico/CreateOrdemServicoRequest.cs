using Ofichina.Contracts.Requests.OrdemServicos;

namespace Ofichina.Contracts.Requests.Cliente;

/// <summary>
/// Requisição para cadastro de nova ordem de serviço.
/// </summary>
public sealed class CreateOrdemServicoRequest : CreateRequest
{
    /// <summary>
    /// Cliente proprietário do veículo.
    /// </summary>
    public Guid ClienteId { get; init; }

    /// <summary>
    /// Veículo que receberá o atendimento.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Funcionário responsável pelo atendimento.
    /// </summary>
    public Guid ResponsavelId { get; init; }

    /// <summary>
    /// Quilometragem atual do veículo na entrada.
    /// </summary>
    public int QuilometragemEntrada { get; init; }

    /// <summary>
    /// Descrição do problema informado pelo cliente.
    /// </summary>
    public string ProblemaRelatado { get; init; } = string.Empty;

    /// <summary>
    /// Observações iniciais da ordem de serviço.
    /// </summary>
    public string? Observacoes { get; init; }

    /// <summary>
    /// Serviços inicialmente previstos.
    /// </summary>
    public ICollection<CreateOrdemServicoItemServicoRequest> Servicos { get; init; } = [];

    /// <summary>
    /// Peças inicialmente previstas.
    /// </summary>
    public ICollection<CreateOrdemServicoItemPecaRequest> Pecas { get; init; } = [];
}