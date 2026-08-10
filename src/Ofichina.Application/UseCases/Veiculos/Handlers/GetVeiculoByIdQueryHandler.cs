using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Veiculos.Mappings;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Application.UseCases.Veiculos.Handlers;

/// <summary>
/// Handler para obter um veículo por Id.
/// </summary>
public sealed class GetVeiculoByIdQueryHandler : IQueryHandler<GetVeiculoByIdQuery, Result<VeiculoResponse>>
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly ILogger<GetVeiculoByIdQueryHandler> _logger;

    public GetVeiculoByIdQueryHandler(
        IVeiculoRepository veiculoRepository,
        ILogger<GetVeiculoByIdQueryHandler> logger)
    {
        _veiculoRepository = veiculoRepository;
        _logger = logger;
    }

    public async Task<Result<VeiculoResponse>> HandleAsync(GetVeiculoByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var veiculo = await _veiculoRepository.GetByIdAsync(query.Id, includePessoa: true, cancellationToken);

            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure<VeiculoResponse>("Veículo não encontrado.");

            return Result.Success(veiculo.ToResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter veículo por Id. VeiculoId: {VeiculoId}", query.Id);
            return Result.Failure<VeiculoResponse>("Não foi possível obter o veículo.");
        }
    }
}


