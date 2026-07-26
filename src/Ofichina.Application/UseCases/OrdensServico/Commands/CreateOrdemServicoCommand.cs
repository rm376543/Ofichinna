using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.OrdensServico;
namespace Ofichina.Application.UseCases.OrdensServico.Commands;

/// <summary>
/// Comando para criação de ordem de serviço.
/// </summary>
public sealed class CreateOrdemServicoCommand : ICommand<Result>
{
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
    /// Problema relatado na abertura da ordem de serviço.
    /// </summary>
    public string ProblemaRelatado { get; init; }

    /// <summary>
    /// Observações iniciais da ordem de serviço.
    /// </summary>
    public string? Observacoes { get; init; }

    public CreateOrdemServicoCommand(CreateOrdemServicoRequest request)
    {
        PessoaId = request.PessoaId;
        VeiculoId = request.VeiculoId;
        FuncionarioId = request.FuncionarioId;
        HodometroEntrada = request.HodometroEntrada;
        ProblemaRelatado = request.ProblemaRelatado;
        Observacoes = request.Observacoes;
    }

}
