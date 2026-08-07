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
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Nome do serviço.
    /// </summary>
    public string Nome { get; init; }

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
    public bool Ativo { get; init; }

    public UpdateServicoCommand(UpdateServicoRequest updateServicoRequest)
    {
        ServicoId = updateServicoRequest.ServicoId;
        Nome = updateServicoRequest.Nome;
        Descricao = updateServicoRequest.Descricao;
        Valor = updateServicoRequest.Valor;
        Ativo = updateServicoRequest.Ativo;
    }
}