using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Pecas.Commands;

/// <summary>
/// Comando para atualização de peça.
/// </summary>
public sealed class UpdatePecaCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da peça.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Nome da peça.
    /// </summary>
    public string Nome { get; init; } = string.Empty;

    /// <summary>
    /// Descrição da peça.
    /// </summary>
    public string? Descricao { get; init; }

    /// <summary>
    /// Código interno da peça.
    /// </summary>
    public string Codigo { get; init; } = string.Empty;

    /// <summary>
    /// Valor unitário da peça.
    /// </summary>
    public decimal Valor { get; init; }

    /// <summary>
    /// Quantidade em estoque.
    /// </summary>
    public int QuantidadeEstoque { get; init; }

    /// <summary>
    /// Indica se a peça está ativa.
    /// </summary>
    public bool Ativo { get; init; } = true;
}