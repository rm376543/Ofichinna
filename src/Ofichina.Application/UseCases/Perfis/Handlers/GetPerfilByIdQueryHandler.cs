using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Perfis;
using Ofichina.Application.Abstractions.Interfaces;

namespace Ofichina.Application.UseCases.Perfis.Handlers;

/// <summary>
/// Handler para obter um perfil por ID.
/// </summary>
public class GetPerfilByIdQueryHandler : IQueryHandler<GetPerfilByIdQuery, Result<PerfilResponse>>
{
    private readonly IPerfilRepository _repository;
    private readonly ILogger<GetPerfilByIdQueryHandler> _logger;

    public GetPerfilByIdQueryHandler(IPerfilRepository repository, ILogger<GetPerfilByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PerfilResponse>> HandleAsync(GetPerfilByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a obtenção do perfil por ID: {Id}", query.Id);
            var perfil = await _repository.GetByIdAsync(query.Id, cancellationToken);

            if (perfil is null)
            {
                _logger.LogWarning("Perfil não encontrado para o ID: {Id}", query.Id);
                return Result.Failure<PerfilResponse>("Perfil não encontrado.");
            }

            return Result.Success(new PerfilResponse
            {
                Id = perfil.Id,
                Nome = perfil.NomePerfil,
                Descricao = perfil.Descricao,
                CreatedAt = perfil.CreatedAt,
                UpdatedAt = perfil.UpdatedAt,
                DeletedAt = perfil.DeletedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter perfil por ID: {Id}", query.Id);
            return Result.Failure<PerfilResponse>("Erro ao obter perfil.");
        }

    }
}
