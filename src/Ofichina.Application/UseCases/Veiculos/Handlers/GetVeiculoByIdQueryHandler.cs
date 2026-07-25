using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Veiculo;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Domain.Entities;

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
            var veiculo = await _veiculoRepository.GetByIdWithPessoaAsync(query.Id, cancellationToken);

            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure<VeiculoResponse>("Veículo não encontrado.");

            return Result.Success(Mapear(veiculo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter veículo por Id. VeiculoId: {VeiculoId}", query.Id);
            return Result.Failure<VeiculoResponse>("Não foi possível obter o veículo.");
        }
    }

    private static VeiculoResponse Mapear(Veiculo veiculo)
    {
        return new VeiculoResponse
        {
            Id = veiculo.Id,
            Placa = veiculo.Placa.ToString(),
            Marca = veiculo.Marca,
            Modelo = veiculo.Modelo,
            AnoFabricacao = veiculo.AnoFabricacao,
            Cor = veiculo.Cor,
            Hodometro = veiculo.Hodometro.Valor,
            HodometroFormatado = veiculo.Hodometro.ToString(),
            CreatedAt = veiculo.CreatedAt,
            UpdatedAt = veiculo.UpdatedAt,
            DeletedAt = veiculo.DeletedAt
        };
    }
}


