using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um item de serviço previsto dentro de um orçamento.
/// </summary>
public class ItemOrcamento : Entity
{
    private readonly List<ItemOrcamentoPeca> _pecas = [];

    public Guid OrcamentoId { get; private set; }

    public Orcamento? Orcamento { get; private set; }

    public Guid ServicoId { get; private set; }

    public Servico? Servico { get; private set; }

    public IReadOnlyCollection<ItemOrcamentoPeca> Pecas => _pecas.AsReadOnly();

    public decimal ValorServico => Servico?.Valor ?? 0m;

    public decimal ValorTotal => ValorServico + _pecas.Sum(x => x.ValorTotal);

    private ItemOrcamento()
    {
    }

    public ItemOrcamento(Guid orcamentoId, Guid servicoId)
    {
        if (orcamentoId == Guid.Empty)
            throw new DomainException("Orçamento obrigatório.");

        if (servicoId == Guid.Empty)
            throw new DomainException("Serviço obrigatório.");

        OrcamentoId = orcamentoId;
        ServicoId = servicoId;
    }

    public ItemOrcamentoPeca AdicionarPeca(Guid pecaId, int quantidade)
    {
        ValidarNaoExcluido();

        var peca = new ItemOrcamentoPeca(Id, pecaId, quantidade);
        _pecas.Add(peca);

        AtualizarDataModificacao();

        return peca;
    }

    public void AtualizarServico(Guid servicoId)
    {
        ValidarNaoExcluido();

        if (servicoId == Guid.Empty)
            throw new DomainException("Serviço obrigatório.");

        ServicoId = servicoId;

        AtualizarDataModificacao();
    }

    public void RemoverPeca(Guid itemOrcamentoPecaId)
    {
        ValidarNaoExcluido();

        var item = _pecas.FirstOrDefault(x => x.Id == itemOrcamentoPecaId);
        if (item is null || item.EstaExcluida())
            throw new DomainException("Peça não encontrada.");

        item.Excluir();
        AtualizarDataModificacao();
    }

    private void ValidarNaoExcluido()
    {
        if (EstaExcluida())
            throw new DomainException("Não é possível alterar um item de orçamento removido.");
    }
}
