

using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Servicos.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Servicos;

namespace Ofichina.Application.UseCases.Servicos.Handlers;

/// <summary>
/// Handler para listar serviços.
/// </summary>
public sealed class GetAllServicosPaginadosQueryHandler : IQueryHandler<GetAllServicosPaginadosQuery, Result<PagedResponse<ServicoResponse>>>
{
    private readonly IServicoRepository _servicoRepository;
    private readonly ILogger<GetAllServicosPaginadosQueryHandler> _logger;

    public GetAllServicosPaginadosQueryHandler(
        IServicoRepository servicoRepository,
        ILogger<GetAllServicosPaginadosQueryHandler> logger)
    {
        _servicoRepository = servicoRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<ServicoResponse>>> HandleAsync(GetAllServicosPaginadosQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var servicos = await _servicoRepository.GetAllServicosPaginadosAsync(query.Pagination, cancellationToken);

            var resultado = servicos.ToPagedResponse(s => new ServicoResponse
            {
                Id = s.Id,
                Nome = s.Nome,
                Descricao = s.Descricao,
                Valor = s.Valor,
                Ativo = !s.EstaExcluida()
            });

            return Result.Success(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar serviços.");
            return Result.Failure<PagedResponse<ServicoResponse>>("Não foi possível obter os serviços.");
        }
    }
}
