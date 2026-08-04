using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Checklist;

namespace Ofichina.Application.UseCases.Checklists.Commands;

/// <summary>
/// Comando para criação de checklist.
/// </summary>
public sealed class CreateChecklistCommand : ICommand<Result>
{
    public Guid VeiculoId { get; init; }

    public Guid PessoaId { get; init; }

    public int HodometroEntrada { get; init; }

    public string ItensVerificados { get; init; } = string.Empty;

    public string? Observacoes { get; init; }

    public CreateChecklistCommand(CreateChecklistRequest request)
    {
        VeiculoId = request.VeiculoId;
        PessoaId = request.PessoaId;
        HodometroEntrada = request.HodometroEntrada;
        ItensVerificados = request.ItensVerificados;
        Observacoes = request.Observacoes;
    }
}