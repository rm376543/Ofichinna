using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

/// <summary>
/// Comando para criação de um item de serviço em um orçamento.
/// </summary>
public sealed class CreateItemOrcamentoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador do orçamento.
    /// </summary>
    public Guid OrcamentoId { get; init; }

    /// <summary>
    /// Serviço executado no orçamento.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Peça utilizada no serviço (opcional).
    /// </summary>
    public Guid? PecaId { get; init; }

    /// <summary>
    /// Quantidade de peças utilizadas.
    /// </summary>
    public int Quantidade { get; init; }

    public CreateItemOrcamentoCommand(CreateItemOrcamentoRequest request)
    {
        OrcamentoId = request.OrcamentoId;
        ServicoId = request.ServicoId;
        PecaId = request.PecaId;
        Quantidade = request.Quantidade;
    }
}
