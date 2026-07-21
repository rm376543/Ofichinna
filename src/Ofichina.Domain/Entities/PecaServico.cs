using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa uma peça vinculada a um serviço da ordem de serviço.
/// </summary>
public class PecaServico : Entity
{
    /// <summary>
    /// Identificador da peça relacionada ao item.
    /// </summary>
    public Guid PecaId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Navegação para a peça cadastrada.
    /// </summary>
    public Peca? Peca { get; private set; }

    /// <summary>
    /// Identificador do item de serviço ao qual a peça pertence.
    /// </summary>
    public Guid ItemServicoId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Descrição da peça no momento da inclusão na ordem.
    /// </summary>
    public string Descricao { get; private set; } = string.Empty;

    /// <summary>
    /// Quantidade de peças utilizadas ou previstas na ordem de serviço.
    /// </summary>
    public int Quantidade { get; private set; } = 0;

    /// <summary>
    /// Valor unitário da peça no momento da inclusão na ordem.
    /// </summary>
    public decimal ValorUnitario { get; private set; } = 0;

    /// <summary>
    /// Valor total calculado da peça.
    /// </summary>
    public decimal ValorTotal => Quantidade * ValorUnitario;

    /// <summary>
    /// Indica se a peça já foi aplicada/utilizada no veículo.
    /// </summary>
    public bool Utilizada { get; private set; }

    /// <summary>
    /// Data em que a peça foi marcada como utilizada.
    /// </summary>
    public DateTime? DataUtilizacao { get; private set; }

    private PecaServico()
    {
    }

    internal PecaServico(
        Guid itemServicoId,
        Guid pecaId,
        string descricao,
        int quantidade,
        decimal valorUnitario)
    {
        if (itemServicoId == Guid.Empty)
            throw new DomainException("Item de serviço obrigatório.");

        if (pecaId == Guid.Empty)
            throw new DomainException("Peça obrigatória.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("Descrição da peça obrigatória.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade inválida.");

        if (valorUnitario <= 0)
            throw new DomainException("Valor inválido.");

        ItemServicoId = itemServicoId;
        PecaId = pecaId;
        Descricao = descricao.Trim();
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
    }

    /// <summary>
    /// Marca a peça como utilizada no veículo.
    /// </summary>
    public void MarcarComoUtilizada()
    {
        if (Utilizada)
            throw new DomainException("A peça já foi utilizada.");

        Utilizada = true;
        DataUtilizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza os dados da peça vinculada ao item.
    /// </summary>
    public void AtualizarDados(
        Guid pecaId,
        string descricao,
        int quantidade,
        decimal valorUnitario)
    {
        if (Utilizada)
            throw new DomainException("Não é possível alterar uma peça já utilizada.");

        if (pecaId == Guid.Empty)
            throw new DomainException("Peça obrigatória.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("Descrição da peça obrigatória.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade inválida.");

        if (valorUnitario <= 0)
            throw new DomainException("Valor inválido.");

        PecaId = pecaId;
        Descricao = descricao.Trim();
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;

        AtualizarDataModificacao();
    }

    /// <summary>
    /// Valida se a peça pode ser removida da ordem de serviço.
    /// </summary>
    public void ValidarRemocao()
    {
        if (Utilizada)
            throw new DomainException("Não é possível remover uma peça já utilizada.");
    }
}
