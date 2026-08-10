using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta com os dados de um agendamento.
/// </summary>
public sealed class AgendamentoResponse : BaseResponse
{
    public Guid AgendamentoId { get; set; }

    public Guid PessoaId { get; set; }

    public string ClienteNome { get; set; } = string.Empty;

    public Guid? DiaId { get; set; }

    public Guid? HorarioId { get; set; }

    public Guid? ConsultorId { get; set; }

    public string ConsultorNome { get; set; } = string.Empty;

    public Guid VeiculoId { get; set; }

    public string VeiculoPlaca { get; set; } = string.Empty;

    public string VeiculoDescricao { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Descricao { get; set; }
}