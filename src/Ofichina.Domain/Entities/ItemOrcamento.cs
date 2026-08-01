using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um item previsto dentro de um orçamento.
/// </summary>
public class ItemOrcamento : Entity
{
    public Guid OrcamentoId { get; private set; }

    public Guid? ServicoId { get; private set; }

    public Servico? Servico { get; private set; }

    public Guid? PecaId { get; private set; }

    public Peca? Peca { get; private set; }

    public int Quantidade { get; private set; }

    private ItemOrcamento()
    {
    }

    public ItemOrcamento(Guid orcamentoId, Guid servicoId, Guid pecaId, int quantidade)
    {
        if (orcamentoId == Guid.Empty)
            throw new DomainException("Orçamento obrigatório.");

        if (servicoId == Guid.Empty && pecaId == Guid.Empty)
            throw new DomainException("Serviço ou peça obrigatória.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade inválida.");

        OrcamentoId = orcamentoId;
        ServicoId = servicoId == Guid.Empty ? null : servicoId;
        PecaId = pecaId == Guid.Empty ? null : pecaId;
        Quantidade = quantidade;
    }

    public void AtualizarDados(Guid servicoId, Guid pecaId, int quantidade)
    {
        ValidarNaoExcluido();

        if (servicoId == Guid.Empty && pecaId == Guid.Empty)
            throw new DomainException("Serviço ou peça obrigatória.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade inválida.");

        ServicoId = servicoId;
        PecaId = pecaId;
        Quantidade = quantidade;

        AtualizarDataModificacao();
    }

    private void ValidarNaoExcluido()
    {
        if (EstaExcluida())
            throw new DomainException("Não é possível alterar um item de orçamento removido.");
    }
}
