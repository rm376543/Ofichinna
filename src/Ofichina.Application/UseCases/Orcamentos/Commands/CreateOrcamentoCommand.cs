using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para criação de orçamento.
/// </summary>
public sealed class CreateOrcamentoCommand : ICommand<Result>
{
    public Guid PessoaId { get; init; }

    public Guid VeiculoId { get; init; }

    public Guid AgendamentoId { get; init; }

    public Guid MecanicoId { get; init; }

    public Guid ConsultorId { get; init; }

    public DateOnly DataValidade { get; init; }

    public string? Observacoes { get; init; }

    public CreateOrcamentoCommand(CreateOrcamentoRequest request)
    {
        PessoaId = request.PessoaId;
        VeiculoId = request.VeiculoId;
        AgendamentoId = request.AgendamentoId;
        MecanicoId = request.MecanicoId;
        ConsultorId = request.ConsultorId;
        DataValidade = request.DataValidade;
        Observacoes = request.Observacoes;
    }
}
