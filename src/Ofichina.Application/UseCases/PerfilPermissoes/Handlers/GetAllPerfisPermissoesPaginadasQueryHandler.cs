using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.PerfilPermissoes.Mappings;
using Ofichina.Application.UseCases.PerfilPermissoes.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.PerfilPermissoes;

namespace Ofichina.Application.UseCases.PerfilPermissoes.Handlers;

public sealed class GetAllPerfisPermissoesPaginadasQueryHandler : IQueryHandler<GetAllPerfisPermissoesPaginadasQuery, Result<PagedResponse<PerfilPermissaoResponse>>>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IPerfilPermissaoRepository _perfilPermissaoRepository;
    private readonly ILogger<GetAllPerfisPermissoesPaginadasQueryHandler> _logger;

    public GetAllPerfisPermissoesPaginadasQueryHandler(
        IPerfilRepository perfilRepository,
        IPerfilPermissaoRepository perfilPermissaoRepository,
        ILogger<GetAllPerfisPermissoesPaginadasQueryHandler> logger)
    {
        _perfilRepository = perfilRepository;
        _perfilPermissaoRepository = perfilPermissaoRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<PerfilPermissaoResponse>>> HandleAsync(GetAllPerfisPermissoesPaginadasQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var perfilExistente = await _perfilRepository.GetByIdAsync(query.PerfilId, cancellationToken);

            if (perfilExistente is null)
                return Result.Failure<PagedResponse<PerfilPermissaoResponse>>("Perfil não encontrado.");

            var permissoes = await _perfilPermissaoRepository.GetAllPermissoesAssociadosDeUmPerfil(query.PerfilId, query.Pagination, cancellationToken);

            var resultado = permissoes.ToPagedResponse(p => p.ToResponse());

            return Result.Success(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar permissões do perfil. PerfilId: {PerfilId}", query.PerfilId);
            return Result.Failure<PagedResponse<PerfilPermissaoResponse>>("Não foi possível obter as permissões do perfil.");
        }
    }
}
