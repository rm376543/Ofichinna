using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

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
    /// Serviço executado na ordem.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Peça utilizada no serviço.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade de peças utilizadas.
    /// </summary>
    public int Quantidade { get; init; }
}
