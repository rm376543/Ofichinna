using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pecas.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pecas;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;
using Ofichina.Contracts;

namespace Ofichina.Application.UseCases.Pecas.Handlers;

/// <summary>
/// Handler para listar peças.
/// </summary>
public sealed class GetAllPecasPaginadasQueryHandler : IQueryHandler<GetAllPecasPaginadasQuery, Result<PagedResponse<PecaResponse>>>
{
    private readonly IPecaRepository _pecaRepository;
    private readonly ILogger<GetAllPecasPaginadasQueryHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instância do handler de listagem de peças.
    /// </summary>
    public GetAllPecasPaginadasQueryHandler(
        IPecaRepository pecaRepository,
        ILogger<GetAllPecasPaginadasQueryHandler> logger)
    {
        _pecaRepository = pecaRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResponse<PecaResponse>>> HandleAsync(GetAllPecasPaginadasQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var pecas = await _pecaRepository.GetPagedAsync(query.Pagination, cancellationToken);

            if (pecas == null)
            {
                _logger.LogWarning("Nenhuma peça encontrada.");
                return Result.Failure<PagedResponse<PecaResponse>>("Nenhuma peça encontrada.");
            }

            var resultado = pecas.ToPagedResponse(p => new PecaResponse
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Codigo = p.Codigo,
                Valor = p.Valor,
                QuantidadeEstoque = p.QuantidadeEstoque,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                DeletedAt = p.DeletedAt
            });

            return Result.Success(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar peças.");
            return Result.Failure<PagedResponse<PecaResponse>>("Não foi possível obter as peças.");
        }
    }
}
