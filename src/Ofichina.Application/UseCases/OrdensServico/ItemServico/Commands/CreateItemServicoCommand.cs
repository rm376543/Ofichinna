using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.OrdensServico.ItemServico.Commands;

/// <summary>
/// Comando para criação de um item de serviço em uma ordem de serviço.
/// </summary>
public sealed class CreateItemServicoCommand : ICommand<Result<Guid>>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Descrição do serviço.
    /// </summary>
    public string Descricao { get; init; } = string.Empty;

    /// <summary>
    /// Valor cobrado pelo serviço.
    /// </summary>
    public decimal Valor { get; init; }
}
