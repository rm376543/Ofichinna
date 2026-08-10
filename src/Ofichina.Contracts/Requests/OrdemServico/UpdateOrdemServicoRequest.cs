using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.OrdensServico;

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
    /// Pessoa proprietária do veículo.
    /// </summary>
    public Guid PessoaId { get; init; }

    /// <summary>
    /// Veículo que receberá o atendimento.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Consultor responsável pelo atendimento.
    /// </summary>
    public Guid ConsultorId { get; init; }

    /// <summary>
    /// Hodometro atual do veículo na entrada.
    /// </summary>
    public int Hodometro { get; init; }

    /// <summary>
    /// Descrição do problema informado pela pessoa.
    /// </summary>
    public string ProblemaRelatado { get; init; } = string.Empty;

    /// <summary>
    /// Observações da ordem de serviço.
    /// </summary>
    public string? Observacoes { get; init; }
}