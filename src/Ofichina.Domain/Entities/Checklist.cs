using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa o checklist de entrada do veículo.
/// </summary>
public class Checklist : Entity
{
    public Guid AgendamentoId { get; private set; }

    public Agendamento? Agendamento { get; private set; }

    public string ItensVerificados { get; private set; } = string.Empty;

    public string? Observacoes { get; private set; }

    public bool Finalizado { get; private set; }

    private Checklist()
    {
    }

    public Checklist(Guid agendamentoId, string itensVerificados, string? observacoes)
    {
        if (agendamentoId == Guid.Empty)
            throw new DomainException("Agendamento obrigatório.");

        AgendamentoId = agendamentoId;
        ItensVerificados = itensVerificados ?? string.Empty;
        Observacoes = observacoes;
        Finalizado = false;
    }

    public void Finalizar()
    {
        if (Finalizado)
            throw new DomainException("O checklist já foi finalizado.");

        Finalizado = true;
        AtualizarDataModificacao();
    }

    public void VincularAgendamento(Guid agendamentoId)
    {
        if (agendamentoId == Guid.Empty)
            throw new DomainException("O agendamento deve ser informado.");

        AgendamentoId = agendamentoId;
        AtualizarDataModificacao();
    }

    public void Reabrir()
    {
        Finalizado = false;
    }

    public bool EstaFinalizado()
    {
        return Finalizado;
    }
}
