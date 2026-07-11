using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Contracts.Responses.Perfil;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Perfis.Handlers;

/// <summary>
/// Handler para obter um perfil por ID.
/// </summary>
public class GetPerfilByIdQueryHandler : IQueryHandler<GetPerfilByIdQuery, PerfilResponse?>
{
    private readonly IPerfilRepository _repository;

    public GetPerfilByIdQueryHandler(IPerfilRepository repository)
    {
        _repository = repository;
    }

    public async Task<PerfilResponse?> HandleAsync(GetPerfilByIdQuery query)
    {
        var perfil = await _repository.GetByIdAsync(query.Id);

        if (perfil is null)
        {
            return null;
        }

        return new PerfilResponse
        {
            Id = perfil.Id,
            Nome = perfil.NomePerfil,
            Descricao = perfil.Descricao,
            CreatedAt = perfil.CreatedAt,
            UpdatedAt = perfil.UpdatedAt,
            DeletedAt = perfil.DeletedAt
        };
    }
}