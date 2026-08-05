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

    public Guid ChecklistId { get; init; }

    public Guid MecanicoDiagnosticoId { get; init; }

    public Guid ResponsavelId { get; init; }

    public DateTime DataValidade { get; init; }

    public decimal Desconto { get; init; }

    public string? Observacoes { get; init; }

    public CreateOrcamentoCommand(CreateOrcamentoRequest request)
    {
        PessoaId = request.PessoaId;
        VeiculoId = request.VeiculoId;
        ChecklistId = request.ChecklistId;
        MecanicoDiagnosticoId = request.MecanicoDiagnosticoId;
        ResponsavelId = request.ResponsavelId;
        DataValidade = request.DataValidade;
        Desconto = request.Desconto;
        Observacoes = request.Observacoes;
    }
}
