using Ofichina.Application.Abstractions;

namespace Ofichina.Application.UseCases.Agendamentos.Queries;

/// <summary>
/// Query para listar dias disponíveis em um período (mês/ano).
/// </summary>
public sealed class ListarDiasDisponiveisQuery : IQuery<IEnumerable<DiaListaDto>>
{
    public int Mes { get; init; }
    public int Ano { get; init; }
}

/// <summary>
/// DTO com informações do dia disponível.
/// </summary>
public sealed class DiaListaDto
{
    public Guid Id { get; set; }
    public string Data { get; set; } = string.Empty; // YYYY-MM-DD
}
