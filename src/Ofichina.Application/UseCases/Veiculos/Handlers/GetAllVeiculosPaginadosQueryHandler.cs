using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Application.UseCases.Veiculos.Handlers;

/// <summary>
/// Handler para listar veículos.
/// </summary>
public sealed class GetAllVeiculosPaginadosQueryHandler
    : IQueryHandler<GetAllVeiculosPaginadosQuery, Result<Contracts.PagedResponse<VeiculoResponse>>>
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly ILogger<GetAllVeiculosPaginadosQueryHandler> _logger;

    public GetAllVeiculosPaginadosQueryHandler(
        IVeiculoRepository veiculoRepository,
        ILogger<GetAllVeiculosPaginadosQueryHandler> logger)
    {
        _veiculoRepository = veiculoRepository;
        _logger = logger;
    }

    public async Task<Result<Contracts.PagedResponse<VeiculoResponse>>> HandleAsync(
        GetAllVeiculosPaginadosQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Obtendo veículos paginados.");

            var veiculos = await _veiculoRepository.GetAllVeiculosPaged(query.Pagination, cancellationToken);

            if (veiculos is null)
            {
                _logger.LogWarning("Nenhum veículo encontrado.");
                return Result.Failure<Contracts.PagedResponse<VeiculoResponse>> ("Nenhum veículo encontrado.");
            }

            var response = veiculos.ToPagedResponse(v => new VeiculoResponse
            {
                Id = v.Id,
                Placa = v.Placa.ToString(),
                Marca = v.Marca,
                Modelo = v.Modelo,
                AnoFabricacao = v.AnoFabricacao,
                Cor = v.Cor,
                Hodometro = v.Hodometro.Valor,
                HodometroFormatado = v.Hodometro.ToString(),
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt,
                DeletedAt = v.DeletedAt
            });

            _logger.LogInformation("Veículos paginados obtidos com sucesso.");
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter veículos.");
            return Result.Failure<Contracts.PagedResponse<VeiculoResponse>>("Não foi possível obter os veículos.");
        }
    }
}




