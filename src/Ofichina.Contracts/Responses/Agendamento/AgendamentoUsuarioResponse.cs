namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta enxuta com os dados de agendamento de usuário provenientes da view vwAgendamentosUsuario.
/// Não expõe AgendamentosId, PessoaId nem campos de auditoria.
/// </summary>
public sealed class AgendamentoUsuarioResponse
{
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
