using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Contracts.Responses.OrdensServico;

/// <summary>
/// Resposta com os dados de uma ordem de serviço.
/// </summary>
public sealed class OrdemServicoResponse : BaseResponse
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; set; }

    /// <summary>
    /// Identificador da pessoa vinculada.
    /// </summary>
    public Guid PessoaId { get; set; }

    /// <summary>
    /// Identificador do veículo vinculado.
    /// </summary>
    public Guid VeiculoId { get; set; }

    /// <summary>
    /// Identificador do consultor responsável.
    /// </summary>
    public Guid ConsultorId { get; set; }

    /// <summary>
    /// Identificador do mecânico responsável pelo reparo.
    /// </summary>
    public Guid MecanicoId { get; set; }

    /// <summary>
    /// Hodômetro de entrada do veículo.
    /// </summary>
    public int Hodometro { get; set; }

    /// <summary>
    /// Problema relatado na abertura da ordem de serviço.
    /// </summary>
    public string ProblemaRelatado { get; set; } = string.Empty;

    /// <summary>
    /// Status atual da ordem de serviço.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Data de abertura da ordem de serviço.
    /// </summary>
    public DateTime DataAbertura { get; set; }

    /// <summary>
    /// Data de finalização da ordem de serviço.
    /// </summary>
    public DateTime? DataFinalizacao { get; set; }

    /// <summary>
    /// Observação geral da ordem de serviço.
    /// </summary>
    public string? Observacao { get; set; }

    /// <summary>
    /// Valor total da ordem de serviço.
    /// </summary>
    public decimal ValorTotal { get; set; }

    /// <summary>
    /// Serviços vinculados à ordem de serviço.
    /// </summary>
    public ICollection<OrdemServicoItensResponse> Servicos { get; set; } = [];
}
