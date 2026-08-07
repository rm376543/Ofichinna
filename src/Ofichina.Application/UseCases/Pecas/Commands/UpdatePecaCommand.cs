using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Pecas;

namespace Ofichina.Application.UseCases.Pecas.Commands;

/// <summary>
/// Comando para atualização de peça.
/// </summary>
public sealed class UpdatePecaCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da peça.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Nome da peça.
    /// </summary>
    public string Nome { get; init; }

    /// <summary>
    /// Descrição da peça.
    /// </summary>
    public string? Descricao { get; init; }

    /// <summary>
    /// Código interno da peça.
    /// </summary>
    public string Codigo { get; init; }

    /// <summary>
    /// Valor unitário da peça.
    /// </summary>
    public decimal Valor { get; init; }

    /// <summary>
    /// Quantidade em estoque.
    /// </summary>
    public int QuantidadeEstoque { get; init; }

    public UpdatePecaCommand(UpdatePecaRequest request)
    {
        PecaId = request.PecaId;
        Nome = request.Nome;
        Descricao = request.Descricao;
        Codigo = request.Codigo;
        Valor = request.Valor;
        QuantidadeEstoque = request.QuantidadeEstoque;
    }
}