using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Permissoes.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Permissoes;

namespace Ofichina.Application.UseCases.Permissoes.Handlers;

public sealed class GetAllPermissoesPaginadasQueryHandler : IQueryHandler<GetAllPermissoesPaginadasQuery, Result<PagedResponse<PermissaoResponse>>>
{
    private readonly IPermissaoRepository _repository;
    private readonly ILogger<GetAllPermissoesPaginadasQueryHandler> _logger;

    public GetAllPermissoesPaginadasQueryHandler(
        IPermissaoRepository repository,
        ILogger<GetAllPermissoesPaginadasQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<PermissaoResponse>>> HandleAsync(GetAllPermissoesPaginadasQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissoes = await _repository.GetPagedAsync(query.Pagination, cancellationToken);

            var resultado = permissoes.ToPagedResponse(p => new PermissaoResponse
            {
                PermissaoId = p.Id,
                Codigo = p.Codigo,
                Descricao = p.Descricao,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });

            return Result.Success(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar permissões.");
            return Result.Failure<PagedResponse<PermissaoResponse>>("Não foi possível obter as permissões.");
        }
    }
}
