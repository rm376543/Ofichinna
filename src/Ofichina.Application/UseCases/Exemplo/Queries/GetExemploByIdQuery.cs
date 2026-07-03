using Ofichina.Application.Abstractions;

namespace Ofichina.Application.UseCases.Exemplo.Queries;

/// <summary>
/// Query para obter um Exemplo por ID.
/// </summary>
public class GetExemploByIdQuery : IQuery<GetExemploByIdResponse?>
{
    public Guid Id { get; set; }

    public GetExemploByIdQuery(Guid id)
    {
        Id = id;
    }
}

/// <summary>
/// Response para GetExemploByIdQuery.
/// </summary>
public class GetExemploByIdResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
