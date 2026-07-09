using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Responses.Perfil;

namespace Ofichina.Application.UseCases.Perfis.Queries;

/// <summary>
/// Query para listar perfis.
/// </summary>
public class GetPerfisQuery : IQuery<IReadOnlyCollection<PerfilResponse>>
{
}