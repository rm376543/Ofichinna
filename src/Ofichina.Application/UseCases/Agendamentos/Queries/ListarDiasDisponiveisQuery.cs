using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Queries;

/// <summary>
/// Query para listar dias disponíveis em um período (mês/ano).
/// </summary>
public sealed class ListarDiasDisponiveisQuery : IQuery<Result<IEnumerable<DiaDisponibilidadeResponse>>>
{
    public int Mes { get; init; }
    public int Ano { get; init; }
}