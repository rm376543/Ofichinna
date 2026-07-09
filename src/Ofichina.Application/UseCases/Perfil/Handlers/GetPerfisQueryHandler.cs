using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Contracts.Responses.Perfil;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Perfis.Handlers;

/// <summary>
/// Handler para listar perfis.
/// </summary>
public class GetPerfisQueryHandler : IQueryHandler<GetPerfisQuery, IReadOnlyCollection<PerfilResponse>>
{
    private readonly IPerfilRepository _repository;

    public GetPerfisQueryHandler(IPerfilRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<PerfilResponse>> HandleAsync(GetPerfisQuery query)
    {
        var perfis = await _repository.GetAllAsync();

        return perfis
            .Select(perfil => new PerfilResponse
            {
                Id = perfil.Id,
                Codigo = perfil.Codigo,
                Nome = perfil.Nome,
                Descricao = perfil.Descricao,
                Ativo = perfil.Ativo,
                CreatedAt = perfil.CreatedAt,
                UpdatedAt = perfil.UpdatedAt
            })
            .ToList();
    }
}