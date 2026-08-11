using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pessoas.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Application.Abstractions.Interfaces.Repository;

namespace Ofichina.Application.UseCases.Pessoas.Handlers;

/// <summary>
/// Handler para listar pessoas.
/// </summary>
public sealed class GetAllPessoasPaginadasQueryHandler : IQueryHandler<GetAllPessoasPaginadasQuery, Result<PagedResponse<PessoaResponse>>>
{
    private readonly IPessoaRepository _repository;
    private readonly ILogger<GetAllPessoasPaginadasQueryHandler> _logger;

    public GetAllPessoasPaginadasQueryHandler(IPessoaRepository repository, ILogger<GetAllPessoasPaginadasQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<PessoaResponse>>> HandleAsync(GetAllPessoasPaginadasQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a obtenção de todas as pessoas.");

            var pessoas = await _repository.GetPagedAsync(query.Pagination, cancellationToken);

            var resultado = pessoas.ToPagedResponse(p => new PessoaResponse
            {
                PessoaId = p.Id,
                Nome = p.Nome,
                Documento = p.Documento.Numero,
                Logradouro = p.Endereco.Logradouro,
                Numero = p.Endereco.Numero,
                Complemento = p.Endereco.Complemento,
                Bairro = p.Endereco.Bairro,
                Cidade = p.Endereco.Cidade,
                Estado = p.Endereco.Estado,
                Cep = p.Endereco.Cep.Formatado,
                Telefone = p.Telefone.Value,
            });

            _logger.LogInformation("Consulta Realizada com Sucesso");

            return Result.Success(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as pessoas.");
            return Result.Failure<PagedResponse<PessoaResponse>>(ex.Message);
        }
    }
}

