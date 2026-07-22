using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

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
    /// Identificador da pessoa vinculada.
    /// </summary>
    public Guid PessoaId { get; init; }

    /// <summary>
    /// Identificador do veículo vinculado.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Identificador do funcionário responsável.
    /// </summary>
    public Guid FuncionarioId { get; init; }

    /// <summary>
    /// Hodômetro de entrada do veículo.
    /// </summary>
    public int HodometroEntrada { get; init; }

    /// <summary>
    /// Problema relatado atualizado.
    /// </summary>
    public string ProblemaRelatado { get; init; } = string.Empty;

    /// <summary>
    /// Observações atualizadas da ordem de serviço.
    /// </summary>
    public string? Observacoes { get; init; }

}
