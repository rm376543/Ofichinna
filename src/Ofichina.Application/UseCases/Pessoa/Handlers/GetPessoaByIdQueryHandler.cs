using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pessoas.Mappings;
using Ofichina.Application.UseCases.Pessoas.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;

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

            return Result.Success(pessoa.ToResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pessoa por Id: {PessoaId}.", query.Id);
            return Result.Failure<PessoaResponse>("Erro ao obter a pessoa.");
        }
    }
}

