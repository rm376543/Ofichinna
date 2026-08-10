namespace Ofichina.Domain.Entities;

/// <summary>
/// Entidade keyless mapeada para a view vwAgendamentosUsuario.
/// Representa os dados de agendamentos de usuários prontos para consulta.
/// Não herda de Entity para evitar aplicação de query filter de soft delete.
/// </summary>
public class AgendamentoUsuarioView
{
    public Guid AgendamentosId { get; set; }

    public Guid PessoaId { get; set; }

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

    public string CreatedAt { get; set; } = string.Empty;

    public string? UpdatedAt { get; set; }

    public string? DeletedAt { get; set; }
}
