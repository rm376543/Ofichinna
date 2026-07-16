using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Servicos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Servicos;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Servicos.Handlers;

/// <summary>
/// Handler para obter um serviço por identificador.
/// </summary>
public sealed class GetServicoByIdQueryHandler : IQueryHandler<GetServicoByIdQuery, Result<ServicoResponse>>
{
    private readonly IRepository<Servico> _servicoRepository;
    private readonly ILogger<GetServicoByIdQueryHandler> _logger;

    public GetServicoByIdQueryHandler(
        IRepository<Servico> servicoRepository,
        ILogger<GetServicoByIdQueryHandler> logger)
    {
        _servicoRepository = servicoRepository;
        _logger = logger;
    }

    public async Task<Result<ServicoResponse>> HandleAsync(GetServicoByIdQuery query)
    {
        try
        {
            var servico = await _servicoRepository.GetByIdAsync(query.Id);

            if (servico is null || servico.EstaExcluida())
                return Result.Failure<ServicoResponse>("Serviço não encontrado.");

            return Result.Success(Mapear(servico));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter serviço por Id. ServicoId: {ServicoId}", query.Id);
            return Result.Failure<ServicoResponse>("Não foi possível obter o serviço.");
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