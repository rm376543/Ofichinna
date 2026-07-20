using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pessoas.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Application.Abstractions.Interfaces;

namespace Ofichina.Application.UseCases.Pessoas.Handlers;

/// <summary>
/// Handler para listar pessoas.
/// </summary>
public sealed class GetPessoaQueryHandler : IQueryHandler<GetPessoasQuery, Result<IReadOnlyCollection<PessoaResponse>>>
{
    private readonly IPessoaRepository _repository;
    private readonly ILogger<GetPessoaQueryHandler> _logger;

    public GetPessoaQueryHandler(IPessoaRepository repository, ILogger<GetPessoaQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<PessoaResponse>>> HandleAsync(GetPessoasQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a obtenção de todas as pessoas.");

            var pessoas = await _repository.GetPagedAsync(query.Pagination, cancellationToken);

            var resultado = pessoas.Items
                .Select(Mapear)
                .ToList();

            _logger.LogInformation("Pessoas obtidas com sucesso. Total de pessoas: {TotalPessoas}", resultado.Count);

            return Result.Success<IReadOnlyCollection<PessoaResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as pessoas.");
            return Result.Failure<IReadOnlyCollection<PessoaResponse>>("Não foi possível obter as pessoas.");
        }
    }

    private static PessoaResponse Mapear(Domain.Entities.Pessoa pessoa)
    {
        return new PessoaResponse
        {
            Id = pessoa.Id,
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
            UsuarioId = pessoa.UsuarioId,
            CreatedAt = pessoa.CreatedAt,
            UpdatedAt = pessoa.UpdatedAt,
            DeletedAt = pessoa.DeletedAt
        };
    }
}

