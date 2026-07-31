using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pessoas.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Pessoas.Handlers;

/// <summary>
/// Handler para obter uma pessoa por Id.
/// </summary>
public sealed class GetPessoaByIdQueryHandler : IQueryHandler<GetPessoaByIdQuery, Result<PessoaResponse>>
{
    private readonly IPessoaRepository _repository;
    private readonly ILogger<GetPessoaByIdQueryHandler> _logger;

    public GetPessoaByIdQueryHandler(IPessoaRepository repository, ILogger<GetPessoaByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PessoaResponse>> HandleAsync(GetPessoaByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a obtenção da pessoa com Id: {PessoaId}.", query.Id);

            var pessoa = await _repository.GetByIdAsync(query.Id, cancellationToken);

            if (pessoa is null || pessoa.EstaExcluida())
            {
                _logger.LogWarning("Pessoa não encontrada. PessoaId: {PessoaId}", query.Id);
                return Result.Failure<PessoaResponse>("Pessoa não encontrada.");
            }

            return Result.Success(Mapear(pessoa));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pessoa por Id: {PessoaId}.", query.Id);
            return Result.Failure<PessoaResponse>("Erro ao obter a pessoa.");
        }
    }

    private static PessoaResponse Mapear(Pessoa pessoa)
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

