using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um serviço executado ou previsto dentro de um orçamento ou ordem de serviço.
/// O vínculo com o agregado é definido pelas chaves estrangeiras opcionais.
/// </summary>
public class ItemServico : Entity
{
    /// <summary>
    /// Identificador do orçamento ao qual o item pertence.
    /// </summary>
    public Guid? OrcamentoId { get; private set; }

    /// <summary>
    /// Navegação para o orçamento.
    /// </summary>
    public Aggregates.Orcamento? Orcamento { get; private set; }

    /// <summary>
    /// Identificador da ordem de serviço à qual o serviço pertence.
    /// </summary>
    public Guid? OrdemServicoId { get; private set; }

    /// <summary>
    /// Navegação para a ordem de serviço.
    /// </summary>
    public Aggregates.OrdemServico? OrdemServico { get; private set; }

    /// <summary>
    /// Identificador do serviço executado na ordem.
    /// </summary>
    public Guid ServicoId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Navegação para o serviço executado.
    /// </summary>
    public Servico? Servico { get; private set; }

    /// <summary>
    /// Valor do serviço associado.
    /// </summary>
    public decimal ValorServico => Servico?.Valor ?? 0m;

    /// <summary>
    /// Identificador da peça utilizada no item de serviço.
    /// </summary>
    public Guid PecaId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Navegação para a peça utilizada.
    /// </summary>
    public Peca? Peca { get; private set; }

    /// <summary>
    /// Valor total do item.
    /// </summary>
    public decimal ValorTotal => ValorServico + ((Peca?.Valor ?? 0m) * Quantidade);

    /// <summary>
    /// Quantidade de peças utilizadas no item de serviço.
    /// </summary>
    public int Quantidade { get; private set; }

    /// <summary>
    /// Construtor utilizado pelo Entity Framework Core.
    /// </summary>
    private ItemServico()
    {
    }

    /// <summary>
    /// Cria um item de serviço vinculado a um orçamento.
    /// </summary>
    public static ItemServico ParaOrcamento(
        Guid orcamentoId,
        Guid servicoId,
        Guid pecaId,
        int quantidade)
    {
        return new ItemServico(orcamentoId, null, servicoId, pecaId, quantidade);
    }

    /// <summary>
    /// Cria um item de serviço vinculado a uma ordem de serviço.
    /// </summary>
    public static ItemServico ParaOrdemServico(
        Guid ordemServicoId,
        Guid servicoId,
        Guid pecaId,
        int quantidade)
    {
        return new ItemServico(null, ordemServicoId, servicoId, pecaId, quantidade);
    }

    private ItemServico(
        Guid? orcamentoId,
        Guid? ordemServicoId,
        Guid servicoId,
        Guid pecaId,
        int quantidade)
    {
        if (orcamentoId is null && ordemServicoId is null)
            throw new DomainException("O item de serviço deve estar vinculado a um orçamento ou a uma ordem de serviço.");

        if (servicoId == Guid.Empty)
            throw new DomainException("Serviço obrigatório.");

        if (pecaId == Guid.Empty)
            throw new DomainException("Peça obrigatória.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade inválida.");

        OrcamentoId = orcamentoId;
        OrdemServicoId = ordemServicoId;
        ServicoId = servicoId;
        PecaId = pecaId;
        Quantidade = quantidade;
    }

    /// <summary>
    /// Atualiza os dados do item de serviço.
    /// </summary>
    public void AtualizarDados(
        Guid servicoId,
        Guid pecaId,
        int quantidade)
    {
        ValidarNaoExcluido();

        if (servicoId == Guid.Empty)
            throw new DomainException("Serviço obrigatório.");

        if (pecaId == Guid.Empty)
            throw new DomainException("Peça obrigatória.");

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
            throw new DomainException("Não é possível alterar um item de serviço removido.");
    }
}
