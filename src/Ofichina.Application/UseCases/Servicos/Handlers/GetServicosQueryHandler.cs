using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Servicos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Servicos;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Servicos.Handlers;

/// <summary>
/// Handler para listar serviços.
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

    public async Task<Result<IReadOnlyCollection<ServicoResponse>>> HandleAsync(GetServicosQuery query)
    {
        try
        {
            var servicos = await _servicoRepository.GetAllAsync();

            var resultado = servicos
                .Where(servico => !servico.EstaExcluida())
                .Select(Mapear)
                .ToList();

            return Result.Success<IReadOnlyCollection<ServicoResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar serviços.");
            return Result.Failure<IReadOnlyCollection<ServicoResponse>>("Não foi possível obter os serviços.");
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
            Ativo = servico.Ativo,
            CreatedAt = servico.CreatedAt,
            UpdatedAt = servico.UpdatedAt,
            DeletedAt = servico.DeletedAt
        };
    }
}