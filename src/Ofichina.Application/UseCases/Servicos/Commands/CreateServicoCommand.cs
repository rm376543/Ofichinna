using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Servicos.Commands;

/// <summary>
/// Comando para criação de serviço.
/// </summary>
public sealed class CreateServicoCommand : ICommand<Result<Guid>>
{
    /// <summary>
    /// Nome do serviço.
    /// </summary>
    public string Nome { get; init; } = string.Empty;

    /// <summary>
    /// Descrição do serviço.
    /// </summary>
    public string? Descricao { get; init; }

    /// <summary>
    /// Valor do serviço.
    /// </summary>
    public decimal Valor { get; init; }

    /// <summary>
    /// Indica se o serviço inicia ativo.
    /// </summary>
    public bool Ativo { get; init; } = true;
}