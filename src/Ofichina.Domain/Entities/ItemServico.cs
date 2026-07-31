using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um serviço executado ou previsto dentro de uma ordem de serviço.
/// Esta entidade pertence ao agregado OrdemServico e seu ciclo de vida
/// é controlado pela própria ordem de serviço.
/// </summary>
public class ItemServico : Entity
{
    /// <summary>
    /// Identificador da ordem de serviço à qual o serviço pertence.
    /// </summary>
    public Guid OrdemServicoId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Identificador do serviço executado na ordem.
    /// </summary>
    public Guid ServicoId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Navegação para o serviço executado.
    /// </summary>
    public Servico? Servico { get; private set; }

    /// <summary>
    /// Identificador da peça utilizada no item de serviço.
    /// </summary>
    public Guid PecaId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Navegação para a peça utilizada.
    /// </summary>
    public Peca? Peca { get; private set; }

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
    /// Cria um item de serviço vinculado a uma ordem de serviço.
    /// </summary>
    public ItemServico(
        Guid ordemServicoId,
        Guid servicoId,
        Guid pecaId,
        int quantidade)
    {
        if (ordemServicoId == Guid.Empty)
            throw new DomainException("Ordem de serviço obrigatória.");

        if (servicoId == Guid.Empty)
            throw new DomainException("Serviço obrigatório.");

        if (pecaId == Guid.Empty)
            throw new DomainException("Peça obrigatória.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade inválida.");

        OrdemServicoId = ordemServicoId;
        ServicoId = servicoId;
        PecaId = pecaId;
        Quantidade = quantidade;
    }

    /// <summary>
    /// Cria um item de serviço vinculado a uma ordem de serviço.
    /// </summary>
    public static ItemServico Criar(
        Guid ordemServicoId,
        Guid servicoId,
        Guid pecaId,
        int quantidade)
    {
        return new ItemServico(ordemServicoId, servicoId, pecaId, quantidade);
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
