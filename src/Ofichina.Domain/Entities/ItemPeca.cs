using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa uma peça adicionada a uma ordem de serviço.
/// A entidade controla a quantidade, valores e o ciclo de utilização da peça.
/// </summary>
public class ItemPeca : Entity
{
    /// <summary>
    /// Identificador da ordem de serviço à qual a peça pertence.
    /// </summary>
    public Guid OrdemServicoId { get; private set; } = Guid.Empty;


    /// <summary>
    /// Identificador do produto/peça utilizado na ordem de serviço.
    /// </summary>
    public Guid ProdutoId { get; private set; } = Guid.Empty;


    /// <summary>
    /// Descrição da peça no momento da inclusão na ordem.
    /// Mantém o histórico mesmo que o cadastro do produto seja alterado.
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
    /// É obtido multiplicando quantidade pelo valor unitário.
    /// </summary>
    public decimal ValorTotal =>
        Quantidade * ValorUnitario;


    /// <summary>
    /// Indica se a peça já foi aplicada/utilizada no veículo.
    /// </summary>
    public bool Utilizada { get; private set; }


    /// <summary>
    /// Data em que a peça foi marcada como utilizada.
    /// </summary>
    public DateTime? DataUtilizacao { get; private set; }


    /// <summary>
    /// Construtor utilizado pelo Entity Framework Core.
    /// </summary>
    private ItemPeca()
    {
    }


    /// <summary>
    /// Cria um item de peça dentro de uma ordem de serviço.
    /// </summary>
    /// <param name="ordemServicoId">
    /// Identificador da ordem de serviço.
    /// </param>
    /// <param name="produtoId">
    /// Identificador do produto/peça.
    /// </param>
    /// <param name="descricao">
    /// Descrição da peça.
    /// </param>
    /// <param name="quantidade">
    /// Quantidade de peças.
    /// </param>
    /// <param name="valorUnitario">
    /// Valor unitário da peça.
    /// </param>
    public ItemPeca(
        Guid ordemServicoId,
        Guid produtoId,
        string descricao,
        int quantidade,
        decimal valorUnitario)
    {
        if (ordemServicoId == Guid.Empty)
            throw new DomainException(
                "Ordem de serviço obrigatória.");


        if (produtoId == Guid.Empty)
            throw new DomainException(
                "Produto obrigatório.");


        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException(
                "Descrição da peça obrigatória.");


        if (quantidade <= 0)
            throw new DomainException(
                "Quantidade inválida.");


        if (valorUnitario <= 0)
            throw new DomainException(
                "Valor inválido.");


        OrdemServicoId = ordemServicoId;
        ProdutoId = produtoId;
        Descricao = descricao;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
    }


    /// <summary>
    /// Marca a peça como utilizada no veículo.
    /// Deve ocorrer durante a execução da ordem de serviço.
    /// </summary>
    public void MarcarComoUtilizada()
    {
        if (Utilizada)
            throw new DomainException(
                "A peça já foi utilizada.");


        Utilizada = true;

        DataUtilizacao = DateTime.UtcNow;
    }


    /// <summary>
    /// Valida se a peça pode ser removida da ordem de serviço.
    /// Peças já utilizadas não podem ser removidas.
    /// </summary>
    public void ValidarRemocao()
    {
        if (Utilizada)
            throw new DomainException(
                "Não é possível remover uma peça já utilizada.");
    }
}