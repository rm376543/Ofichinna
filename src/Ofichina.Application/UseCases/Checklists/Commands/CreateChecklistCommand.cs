using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Checklist;

namespace Ofichina.Application.UseCases.Checklists.Commands;

/// <summary>
/// Comando para criação de checklist.
/// </summary>
public sealed class CreateChecklistCommand : ICommand<Result>
{
    public Guid AgendamentoId { get; init; }

    public string ItensVerificados { get; init; }

    public string? Observacoes { get; init; }

    public CreateChecklistCommand(CreateChecklistRequest request)
    {
        AgendamentoId = request.AgendamentoId;
        ItensVerificados = request.ItensVerificados;
        Observacoes = request.Observacoes;
    }
}