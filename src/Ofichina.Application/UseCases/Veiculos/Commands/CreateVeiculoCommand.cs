using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Veiculo;

namespace Ofichina.Application.UseCases.Veiculos.Commands;

/// <summary>
/// Comando para criação de veículo.
/// </summary>
public sealed class CreateVeiculoCommand : ICommand<Result>
{
    public Guid PessoaId { get; init; }

    public string Placa { get; init; }

    public string Marca { get; init; }

    public string Modelo { get; init; }

    public int AnoFabricacao { get; init; }

    public string? Cor { get; init; }

    public int Hodometro { get; init; }

    public CreateVeiculoCommand(CreateVeiculoRequest request)
    {
        PessoaId = request.PessoaId;
        Placa = request.Placa;
        Marca = request.Marca;
        Modelo = request.Modelo;
        AnoFabricacao = request.AnoFabricacao;
        Cor = request.Cor;
        Hodometro = request.Hodometro;
    }
}