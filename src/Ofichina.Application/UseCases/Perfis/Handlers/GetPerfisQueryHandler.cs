using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Perfis;
using Ofichina.Application.Abstractions.Interfaces;

namespace Ofichina.Application.UseCases.Perfis.Handlers;

/// <summary>
/// Handler para listar perfis.
/// </summary>
public class GetPerfisQueryHandler : IQueryHandler<GetPerfisQuery, Result<IReadOnlyCollection<PerfilResponse>>>
{
    private readonly IPerfilRepository _repository;
    private readonly ILogger<GetPerfisQueryHandler> _logger;

    public GetPerfisQueryHandler(IPerfilRepository repository, ILogger<GetPerfisQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<PerfilResponse>>> HandleAsync(GetPerfisQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a obtenção de todos os perfis.");

            var perfis = await _repository.GetPagedAsync(query.Pagination, cancellationToken);

            var resultado = perfis.Items
                .Select(perfil => new PerfilResponse
                {
                    Id = perfil.Id,
                    Nome = perfil.NomePerfil,
                    Descricao = perfil.Descricao,
                    CreatedAt = perfil.CreatedAt,
                    UpdatedAt = perfil.UpdatedAt,
                    DeletedAt = perfil.DeletedAt
                })
                .ToList();

            _logger.LogInformation("Perfis obtidos com sucesso. Total de perfis: {TotalPerfis}", resultado.Count);

            return Result.Success<IReadOnlyCollection<PerfilResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todos os perfis.");
            return Result.Failure<IReadOnlyCollection<PerfilResponse>>("Não foi possível obter os perfis.");
        }
    }
}
