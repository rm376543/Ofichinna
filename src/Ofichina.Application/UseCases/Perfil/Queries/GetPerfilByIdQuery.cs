using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Responses.Perfil;

namespace Ofichina.Application.UseCases.Perfil.Queries;

/// <summary>
/// Query para obter um perfil por ID.
/// </summary>
public class GetPerfilByIdQuery : IQuery<PerfilResponse?>
{
    public Guid Id { get; set; }

    public GetPerfilByIdQuery(Guid id)
    {
        Id = id;
    }
}