using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Pecas;

namespace Ofichina.Application.UseCases.Pecas.Commands;

/// <summary>
/// Comando para criação de peça.
/// </summary>
public sealed class CreatePecaCommand : ICommand<Result>
{
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
    /// Quantidade inicial em estoque.
    /// </summary>
    public int QuantidadeEstoque { get; init; }

    public CreatePecaCommand(CreatePecaRequest request)
    {
        Nome = request.Nome;
        Descricao = request.Descricao;
        Codigo = request.Codigo;
        Valor = request.Valor;
        QuantidadeEstoque = request.QuantidadeEstoque;
    }
}