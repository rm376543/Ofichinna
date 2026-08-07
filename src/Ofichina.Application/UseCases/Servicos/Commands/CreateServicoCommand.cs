using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Servicos;

namespace Ofichina.Application.UseCases.Servicos.Commands;

/// <summary>
/// Comando para criação de serviço.
/// </summary>
public sealed class CreateServicoCommand : ICommand<Result>
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

    public CreateServicoCommand(CreateServicoRequest request)
    {
        Nome = request.Nome;
        Descricao = request.Descricao;
        Valor = request.Valor;
    }
}