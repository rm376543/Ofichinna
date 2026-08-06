using Ofichina.Application.Abstractions;

namespace Ofichina.Application.UseCases.Agendamentos.Queries;

/// <summary>
/// Query para listar consultores disponíveis para dia + horário.
/// </summary>
public sealed class ListarConsultoresPorDiaHorarioQuery : IQuery<IEnumerable<ConsultorListaDto>>
{
    public Guid DiaDisponibilidadeId { get; init; }
    public Guid HorarioDisponibilidadeId { get; init; }
}

/// <summary>
/// DTO com informações do consultor.
/// </summary>
public sealed class ConsultorListaDto
{
    public Guid PessoaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
}
