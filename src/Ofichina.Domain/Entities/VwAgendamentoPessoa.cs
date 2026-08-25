namespace Ofichina.Domain.Entities;

/// <summary>
/// Entidade keyless mapeada para a view vwAgendamentoPessoa.
/// Representa os dados de agendamentos de usuários prontos para consulta.
/// Herda de Entity para manter o contrato de auditoria do domínio.
/// </summary>
public class VwAgendamentoPessoa : Entity
{
    public Guid AgendamentosId { get; set; }

    public Guid PessoaId { get; set; }

    public Guid VeiculoId { get; set; }

    public string StatusAgendamento { get; set; } = string.Empty;

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

    public DateTime DtAgendamento { get; set; }

    public TimeOnly HorarioAgendamento { get; set; }
}
