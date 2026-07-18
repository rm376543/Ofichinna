using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa uma peça cadastrada no catálogo da aplicação.
/// O cadastro mantém dados comerciais e de estoque para uso em ordens de serviço.
/// </summary>
public class Peca : Entity
{
    /// <summary>
    /// Nome da peça.
    /// </summary>
    public string Nome { get; private set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada da peça.
    /// </summary>
    public string? Descricao { get; private set; }

    /// <summary>
    /// Código interno ou de referência da peça.
    /// </summary>
    public string Codigo { get; private set; } = string.Empty;

    /// <summary>
    /// Valor unitário da peça.
    /// </summary>
    public decimal Valor { get; private set; }

    /// <summary>
    /// Quantidade disponível em estoque.
    /// </summary>
    public int QuantidadeEstoque { get; private set; }

    /// <summary>
    /// Construtor utilizado pelo Entity Framework Core.
    /// </summary>
    private Peca()
    {
    }

    /// <summary>
    /// Cria uma nova peça do catálogo.
    /// </summary>
    /// <param name="nome">Nome da peça.</param>
    /// <param name="descricao">Descrição da peça.</param>
    /// <param name="codigo">Código interno da peça.</param>
    /// <param name="valor">Valor unitário da peça.</param>
    /// <param name="quantidadeEstoque">Quantidade inicial em estoque.</param>
    public Peca(
        string nome,
        string? descricao,
        string codigo,
        decimal valor,
        int quantidadeEstoque)
    {
        ValidarDados(nome, codigo, valor, quantidadeEstoque);

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        Codigo = codigo.Trim();
        Valor = valor;
        QuantidadeEstoque = quantidadeEstoque;
    }

    /// <summary>
    /// Atualiza os dados cadastrais da peça.
    /// </summary>
    /// <param name="nome">Novo nome da peça.</param>
    /// <param name="descricao">Nova descrição da peça.</param>
    /// <param name="codigo">Novo código interno da peça.</param>
    /// <param name="valor">Novo valor unitário.</param>
    /// <param name="quantidadeEstoque">Nova quantidade em estoque.</param>
    public void AtualizarDados(
        string nome,
        string? descricao,
        string codigo,
        decimal valor,
        int quantidadeEstoque)
    {
        ValidarDados(nome, codigo, valor, quantidadeEstoque);

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        Codigo = codigo.Trim();
        Valor = valor;
        QuantidadeEstoque = quantidadeEstoque;

        AtualizarDataModificacao();
    }

    /// <summary>
    /// Ajusta a quantidade em estoque da peça.
    /// </summary>
    /// <param name="novaQuantidade">Nova quantidade em estoque.</param>
    public void AjustarQuantidadeEstoque(int novaQuantidade)
    {
        if (novaQuantidade < 0)
            throw new DomainException("A quantidade em estoque não pode ser negativa.");

        QuantidadeEstoque = novaQuantidade;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Incrementa a quantidade em estoque da peça.
    /// </summary>
    /// <param name="quantidade">Quantidade a adicionar.</param>
    public void EntradaEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade de entrada em estoque deve ser maior que zero.");

        QuantidadeEstoque += quantidade;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Reduz a quantidade em estoque da peça.
    /// </summary>
    /// <param name="quantidade">Quantidade a remover.</param>
    public void SaidaEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade de saída em estoque deve ser maior que zero.");

        if (quantidade > QuantidadeEstoque)
            throw new DomainException("Quantidade insuficiente em estoque.");

        QuantidadeEstoque -= quantidade;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Ativa a peça no catálogo por meio da reativação lógica.
    /// </summary>
    public void Ativar()
    {
        Reativar();
    }

    /// <summary>
    /// Desativa a peça no catálogo por meio de exclusão lógica.
    /// </summary>
    public void Desativar()
    {
        if (EstaExcluida())
            return;

        Excluir();
    }

    /// <summary>
    /// Realiza a exclusão lógica da peça.
    /// </summary>
    public void ExcluirLogicamente()
    {
        Excluir();
    }

    private static void ValidarDados(string nome, string codigo, decimal valor, int quantidadeEstoque)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da peça deve ser informado.");

        if (string.IsNullOrWhiteSpace(codigo))
            throw new DomainException("O código da peça deve ser informado.");

        if (valor <= 0)
            throw new DomainException("O valor da peça deve ser maior que zero.");

        if (quantidadeEstoque < 0)
            throw new DomainException("A quantidade em estoque não pode ser negativa.");
    }
}