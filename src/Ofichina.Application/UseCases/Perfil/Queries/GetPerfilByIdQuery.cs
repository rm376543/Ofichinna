using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Perfil;

namespace Ofichina.Application.UseCases.Perfis.Queries;

/// <summary>
/// Query para obter um perfil por ID.
/// </summary>
public class GetPerfilByIdQuery : IQuery<Result<PerfilResponse>>
{
    public Guid Id { get; set; }

    public GetPerfilByIdQuery(Guid id)
    {
        Id = id;
    }
}