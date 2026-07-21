using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.OrdemServico;

namespace Ofichina.Application.UseCases.OrdensServico.Commands;

/// <summary>
/// Comando para atualização de ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identificador do funcionário responsável.
    /// </summary>
    public Guid FuncionarioId { get; init; }

    /// <summary>
    /// Problema relatado atualizado.
    /// </summary>
    public string ProblemaRelatado { get; init; } = string.Empty;

    /// <summary>
    /// Observações atualizadas da ordem de serviço.
    /// </summary>
    public string? Observacoes { get; init; }

    /// <summary>
    /// Serviços previstos para a ordem de serviço.
    /// </summary>
    public ICollection<UpdateOrdemServicoItemServicoRequest> Servicos { get; init; } = [];

}
