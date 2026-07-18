using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Servicos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Servicos;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Servicos.Handlers;

/// <summary>
/// Handler para listar serviÃ§os.
/// </summary>
public sealed class GetServicosQueryHandler : IQueryHandler<GetServicosQuery, Result<IReadOnlyCollection<ServicoResponse>>>
{
    private readonly IRepository<Servico> _servicoRepository;
    private readonly ILogger<GetServicosQueryHandler> _logger;

    public GetServicosQueryHandler(
        IRepository<Servico> servicoRepository,
        ILogger<GetServicosQueryHandler> logger)
    {
        _servicoRepository = servicoRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<ServicoResponse>>> HandleAsync(GetServicosQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var servicos = await _servicoRepository.GetPagedAsync(query.Pagination, cancellationToken);

            var resultado = servicos.Items
                .Select(Mapear)
                .ToList();

            return Result.Success<IReadOnlyCollection<ServicoResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar serviÃ§os.");
            return Result.Failure<IReadOnlyCollection<ServicoResponse>>("NÃ£o foi possÃ­vel obter os serviÃ§os.");
        }
    }

    private static ServicoResponse Mapear(Servico servico)
    {
        return new ServicoResponse
        {
            Id = servico.Id,
            Nome = servico.Nome,
            Descricao = servico.Descricao,
            Valor = servico.Valor,
            Ativo = !servico.EstaExcluida(),
            CreatedAt = servico.CreatedAt,
            UpdatedAt = servico.UpdatedAt,
            DeletedAt = servico.DeletedAt
        };
    }
}
