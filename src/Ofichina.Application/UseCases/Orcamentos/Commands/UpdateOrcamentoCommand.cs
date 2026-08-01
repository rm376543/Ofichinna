using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Contracts.Requests.Orcamentos;
using Ofichina.Contracts.Requests.Orcamentos;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para atualização de orçamento.
/// </summary>
public sealed class UpdateOrcamentoCommand : ICommand<Result>
{
    public Guid Id { get; init; }

    public Guid PessoaId { get; init; }

    public Guid VeiculoId { get; init; }

    public Guid MecanicoDiagnosticoId { get; init; }

    public Guid ResponsavelId { get; init; }

    public DateTime DataValidade { get; init; }

    public decimal Desconto { get; init; }

    public string? Observacoes { get; init; }

    public ICollection<UpdateOrcamentoServicoRequest> Servicos { get; init; } = [];

    public ICollection<UpdateOrcamentoPecaRequest> Pecas { get; init; } = [];

    public UpdateOrcamentoCommand(UpdateOrcamentoRequest request)
    {
        Id = request.Id;
        PessoaId = request.PessoaId;
        VeiculoId = request.VeiculoId;
        MecanicoDiagnosticoId = request.MecanicoDiagnosticoId;
        ResponsavelId = request.ResponsavelId;
        DataValidade = request.DataValidade;
        Desconto = request.Desconto;
        Observacoes = request.Observacoes;
        Servicos = request.Servicos;
        Pecas = request.Pecas;
    }
}
