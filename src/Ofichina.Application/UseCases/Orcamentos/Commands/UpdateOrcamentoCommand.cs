using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para atualização do cabeçalho de orçamento.
/// </summary>
public sealed class UpdateOrcamentoCommand : ICommand<Result>
{
    public Guid OrcamentoId { get; init; }

    public Guid PessoaId { get; init; }

    public Guid VeiculoId { get; init; }

    public Guid MecanicoId { get; init; }

    public Guid ConsultorId { get; init; }

    public DateOnly DataValidade { get; init; }

    public string? Observacoes { get; init; }

    public UpdateOrcamentoCommand(UpdateOrcamentoRequest request)
    {
        OrcamentoId = request.OrcamentoId;
        PessoaId = request.PessoaId;
        VeiculoId = request.VeiculoId;
        MecanicoId = request.MecanicoId;
        ConsultorId = request.ConsultorId;
        DataValidade = request.DataValidade;
        Observacoes = request.Observacoes;
    }
}
