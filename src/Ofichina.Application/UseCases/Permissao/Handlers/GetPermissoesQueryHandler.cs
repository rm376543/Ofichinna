using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Permissoes.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Permissao;

namespace Ofichina.Application.UseCases.Permissoes.Handlers;

public sealed class GetPermissoesQueryHandler : IQueryHandler<GetPermissoesQuery, Result<IReadOnlyCollection<PermissaoResponse>>>
{
    private readonly IPermissaoRepository _repository;
    private readonly ILogger<GetPermissoesQueryHandler> _logger;

    public GetPermissoesQueryHandler(
        IPermissaoRepository repository,
        ILogger<GetPermissoesQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<PermissaoResponse>>> HandleAsync(GetPermissoesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissoes = (await _repository.GetAllAsync(cancellationToken))
                .OrderBy(x => x.Codigo)
                .Select(x => new PermissaoResponse
                {
                    Id = x.Id,
                    Codigo = x.Codigo,
                    Descricao = x.Descricao
                })
                .ToList();

            return Result.Success<IReadOnlyCollection<PermissaoResponse>>(permissoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar permissões.");
            return Result.Failure<IReadOnlyCollection<PermissaoResponse>>("Não foi possível obter as permissões.");
        }
    }
}
