using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa uma peça associada a um item de serviço do orçamento.
/// </summary>
public class ItemOrcamentoPeca : Entity
{
    public Guid ItemOrcamentoId { get; private set; }

    public Guid PecaId { get; private set; }

    public Peca? Peca { get; private set; }

    public int Quantidade { get; private set; }

    public decimal ValorTotal => (Peca?.Valor ?? 0m) * Quantidade;

    private ItemOrcamentoPeca()
    {
    }

    public ItemOrcamentoPeca(Guid itemOrcamentoId, Guid pecaId, int quantidade)
    {
        if (itemOrcamentoId == Guid.Empty)
            throw new DomainException("Item de orçamento obrigatório.");

        if (pecaId == Guid.Empty)
            throw new DomainException("Peça obrigatória.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade inválida.");

        ItemOrcamentoId = itemOrcamentoId;
        PecaId = pecaId;
        Quantidade = quantidade;
    }
}
