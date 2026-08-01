using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa o checklist de entrada do veículo vinculado ao orçamento.
/// </summary>
public class Checklist : Entity
{
    public Guid OrcamentoId { get; private set; }

    public Orcamento? Orcamento { get; private set; }

    public int HodometroEntrada { get; private set; }

    public string ItensVerificados { get; private set; } = string.Empty;

    public string? Observacoes { get; private set; }

    private Checklist()
    {
    }

    public Checklist(Guid orcamentoId, int hodometroEntrada, string itensVerificados, string? observacoes)
    {
        if (orcamentoId == Guid.Empty)
            throw new DomainException("Orçamento obrigatório.");

        if (hodometroEntrada < 0)
            throw new DomainException("O hodômetro de entrada não pode ser negativo.");

        OrcamentoId = orcamentoId;
        HodometroEntrada = hodometroEntrada;
        ItensVerificados = itensVerificados ?? string.Empty;
        Observacoes = observacoes;
    }
}
