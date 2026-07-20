using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Permissoes.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Permissao;

namespace Ofichina.Application.UseCases.Permissoes.Handlers;

public sealed class GetPermissaoByIdQueryHandler : IQueryHandler<GetPermissaoByIdQuery, Result<PermissaoResponse>>
{
    private readonly IPermissaoRepository _repository;
    private readonly ILogger<GetPermissaoByIdQueryHandler> _logger;

    public GetPermissaoByIdQueryHandler(
        IPermissaoRepository repository,
        ILogger<GetPermissaoByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PermissaoResponse>> HandleAsync(GetPermissaoByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissao = await _repository.GetByIdAsync(query.Id, cancellationToken);

            if (permissao is null)
                return Result.Failure<PermissaoResponse>("Permissão não encontrada.");

            return Result.Success(new PermissaoResponse
            {
                Id = permissao.Id,
                Codigo = permissao.Codigo,
                Descricao = permissao.Descricao
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter permissão. PermissaoId: {PermissaoId}", query.Id);
            return Result.Failure<PermissaoResponse>("Não foi possível obter a permissão.");
        }
    }
}
