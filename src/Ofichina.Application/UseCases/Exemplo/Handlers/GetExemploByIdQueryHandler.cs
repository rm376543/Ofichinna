using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Exemplo.Queries;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Exemplo.Handlers;

/// <summary>
/// Handler para obter um Exemplo por ID.
/// </summary>
public class GetExemploByIdQueryHandler : IQueryHandler<GetExemploByIdQuery, GetExemploByIdResponse?>
{
    private readonly IExemploRepository _repository;

    public GetExemploByIdQueryHandler(IExemploRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetExemploByIdResponse?> HandleAsync(GetExemploByIdQuery query)
    {
        var exemplo = await _repository.GetByIdAsync(query.Id);

        if (exemplo == null)
        {
            return null;
        }

        return new GetExemploByIdResponse
        {
            Id = exemplo.Id,
            Nome = exemplo.Nome,
            Descricao = exemplo.Descricao,
            Ativo = exemplo.Ativo,
            CreatedAt = exemplo.CreatedAt,
            UpdatedAt = exemplo.UpdatedAt
        };
    }
}
