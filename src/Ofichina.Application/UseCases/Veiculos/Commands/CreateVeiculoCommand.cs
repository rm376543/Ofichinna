using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Veiculos.Commands;

/// <summary>
/// Comando para criação de veículo.
/// </summary>
public sealed class CreateVeiculoCommand : ICommand<Result>
{
    public Guid PessoaId { get; init; }

    public string Placa { get; init; } = string.Empty;

    public string Marca { get; init; } = string.Empty;

    public string Modelo { get; init; } = string.Empty;

    public int AnoFabricacao { get; init; }

    public string? Cor { get; init; }

    public int Hodometro { get; init; }
}