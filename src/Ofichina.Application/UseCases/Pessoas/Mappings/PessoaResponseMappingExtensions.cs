using Ofichina.Contracts.Extension;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Pessoas.Mappings;

public static class PessoaResponseMappingExtensions
{
    public static PessoaResponse ToResponse(this Pessoa pessoa)
    {
        ArgumentNullException.ThrowIfNull(pessoa);

        return new PessoaResponse
        {
            PessoaId = pessoa.Id,
            Nome = pessoa.Nome,
            Documento = pessoa.Documento.ToString(),
            Telefone = pessoa.Telefone.ToString(),
            Logradouro = pessoa.Endereco.Logradouro,
            Numero = pessoa.Endereco.Numero,
            Complemento = pessoa.Endereco.Complemento,
            Bairro = pessoa.Endereco.Bairro,
            Cidade = pessoa.Endereco.Cidade,
            Estado = pessoa.Endereco.Estado,
            Cep = pessoa.Endereco.Cep.ToString(),
            CreatedAt = pessoa.CreatedAt.ToDateString(),
            UpdatedAt = pessoa.UpdatedAt?.ToDateString(),
            DeletedAt = pessoa.DeletedAt?.ToDateString()
        };
    }
}
