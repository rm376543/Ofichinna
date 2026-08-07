using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Servicos;

namespace Ofichina.Application.UseCases.Servicos.Commands;

/// <summary>
/// Comando para atualização de serviço.
/// </summary>
public sealed class UpdateServicoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador do serviço.
    /// </summary>
    public Guid Id { get; init; }

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
    /// Indica se o serviço está ativo.
    /// </summary>
    public bool Ativo { get; init; } = true;

    public UpdateServicoCommand(UpdateServicoRequest request)
    {
        Id = request.Id;
        Nome = request.Nome;
        Descricao = request.Descricao;
        Valor = request.Valor;
        Ativo = request.Ativo;
    }
}