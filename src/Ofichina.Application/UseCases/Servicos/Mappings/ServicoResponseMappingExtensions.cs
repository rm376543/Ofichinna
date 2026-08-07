using Ofichina.Contracts.Responses.Servicos;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Servicos.Mappings;

public static class ServicoResponseMappingExtensions
{
    public static ServicoResponse ToResponse(this Servico servico)
    {
        ArgumentNullException.ThrowIfNull(servico);

        return new ServicoResponse
        {
            ServicoId = servico.Id,
            Nome = servico.Nome,
            Descricao = servico.Descricao,
            Valor = servico.Valor,
            Ativo = !servico.EstaExcluida(),
            CreatedAt = servico.CreatedAt,
            UpdatedAt = servico.UpdatedAt,
            DeletedAt = servico.DeletedAt
        };
    }
}
