using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta detalhada com os dados de agendamento de usuário provenientes da view vwAgendamentoPessoa.
/// Expõe os identificadores internos e a auditoria completa para detalhamento.
/// </summary>
public sealed class AgendamentoUsuarioDetalheResponse : BaseResponse
{
    public Guid AgendamentosId { get; set; }

    public Guid PessoaId { get; set; }

    public Guid VeiculoId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Documento { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Placa { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string Modelo { get; set; } = string.Empty;

    public int AnoFabricacao { get; set; }

    public string Cor { get; set; } = string.Empty;

    public int Hodometro { get; set; }

    public string Consultor { get; set; } = string.Empty;

    public string DtAgendamento { get; set; } = string.Empty;

    public TimeOnly HorarioAgendamento { get; set; }
}