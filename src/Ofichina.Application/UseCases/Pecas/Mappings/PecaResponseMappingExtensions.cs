using Ofichina.Contracts.Extension;
using Ofichina.Contracts.Responses.Pecas;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Pecas.Mappings;

public static class PecaResponseMappingExtensions
{
    public static PecaResponse ToResponse(this Peca peca)
    {
        ArgumentNullException.ThrowIfNull(peca);

        return new PecaResponse
        {
            PecaId = peca.Id,
            Nome = peca.Nome,
            Descricao = peca.Descricao,
            Codigo = peca.Codigo,
            Valor = peca.Valor,
            QuantidadeEstoque = peca.QuantidadeEstoque,
            CreatedAt = peca.CreatedAt.ToDateString(),
            UpdatedAt = peca.UpdatedAt?.ToDateString(),
            DeletedAt = peca.DeletedAt?.ToDateString()
        };
    }
}
