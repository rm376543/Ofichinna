using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.OrdensServico;

/// <summary>
/// Requisição para cadastro de nova ordem de serviço.
/// </summary>
public sealed class CreateOrdemServicoRequest : CreateRequest
{
    /// <summary>
    /// Pessoa proprietária do veículo.
    /// </summary>
    public Guid PessoaId { get; init; }

    /// <summary>
    /// Veículo que receberá o atendimento.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Funcionário responsável pelo atendimento.
    /// </summary>
    public Guid FuncionarioId { get; init; }

    /// <summary>
    /// Hodometro atual do veículo na entrada.
    /// </summary>
    public int HodometroEntrada { get; init; }

    /// <summary>
    /// Descrição do problema informado pela pessoa.
    /// </summary>
    public string ProblemaRelatado { get; init; } = string.Empty;

    /// <summary>
    /// Observações iniciais da ordem de serviço.
    /// </summary>
    public string? Observacoes { get; init; }

}