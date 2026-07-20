using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.PerfilPermissoes.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.PerfilPermissao;

namespace Ofichina.Application.UseCases.PerfilPermissoes.Handlers;

public sealed class GetPermissoesDoPerfilQueryHandler : IQueryHandler<GetPermissoesDoPerfilQuery, Result<IReadOnlyCollection<PerfilPermissaoResponse>>>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IPerfilPermissaoRepository _perfilPermissaoRepository;
    private readonly ILogger<GetPermissoesDoPerfilQueryHandler> _logger;

    public GetPermissoesDoPerfilQueryHandler(
        IPerfilRepository perfilRepository,
        IPerfilPermissaoRepository perfilPermissaoRepository,
        ILogger<GetPermissoesDoPerfilQueryHandler> logger)
    {
        _perfilRepository = perfilRepository;
        _perfilPermissaoRepository = perfilPermissaoRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<PerfilPermissaoResponse>>> HandleAsync(GetPermissoesDoPerfilQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var perfilExistente = await _perfilRepository.GetByIdAsync(query.PerfilId, cancellationToken);

            if (perfilExistente is null)
                return Result.Failure<IReadOnlyCollection<PerfilPermissaoResponse>>("Perfil não encontrado.");

            var permissoes = (await _perfilPermissaoRepository.GetByPerfilIdAsync(query.PerfilId, cancellationToken))
                .OrderBy(x => x.Permissao.Codigo)
                .Select(x => new PerfilPermissaoResponse
                {
                    PerfilId = x.PerfilId,
                    PermissaoId = x.PermissaoId,
                    Codigo = x.Permissao.Codigo,
                    Descricao = x.Permissao.Descricao
                })
                .ToList();

            return Result.Success<IReadOnlyCollection<PerfilPermissaoResponse>>(permissoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar permissões do perfil. PerfilId: {PerfilId}", query.PerfilId);
            return Result.Failure<IReadOnlyCollection<PerfilPermissaoResponse>>("Não foi possível obter as permissões do perfil.");
        }
    }
}
