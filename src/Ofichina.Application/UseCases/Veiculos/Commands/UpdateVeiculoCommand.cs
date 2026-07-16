using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Veiculos.Commands;

/// <summary>
/// Comando para atualização de veículo.
/// </summary>
public sealed class UpdateVeiculoCommand : ICommand<Result>
{
    public Guid Id { get; init; }

    public Guid PessoaId { get; init; }

    public string Placa { get; init; } = string.Empty;

    public string Marca { get; init; } = string.Empty;

    public string Modelo { get; init; } = string.Empty;

    public int AnoFabricacao { get; init; }

    public string? Cor { get; init; }

    public string? Observacoes { get; init; }

    public int Hodometro { get; init; }

    public bool Ativo { get; init; } = true;
}