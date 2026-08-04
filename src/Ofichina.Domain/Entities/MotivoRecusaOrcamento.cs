using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Aggregates;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Registra o motivo da recusa de um orçamento.
/// </summary>
public sealed class MotivoRecusaOrcamento : Entity
{
    public Guid OrcamentoId { get; private set; }

    public Orcamento? Orcamento { get; private set; }

    public string? Descricao { get; private set; }

    private MotivoRecusaOrcamento()
    {
    }

    public MotivoRecusaOrcamento(Guid orcamentoId, string? descricao)
    {
        if (orcamentoId == Guid.Empty)
            throw new DomainException("Orçamento obrigatório.");

        OrcamentoId = orcamentoId;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }
}