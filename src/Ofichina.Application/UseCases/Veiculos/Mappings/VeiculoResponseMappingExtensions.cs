using Ofichina.Contracts.Responses.Veiculo;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Veiculos.Mappings;

public static class VeiculoResponseMappingExtensions
{
    public static VeiculoResponse ToResponse(this Veiculo veiculo)
    {
        ArgumentNullException.ThrowIfNull(veiculo);

        return new VeiculoResponse
        {
            VeiculoId = veiculo.Id,
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
