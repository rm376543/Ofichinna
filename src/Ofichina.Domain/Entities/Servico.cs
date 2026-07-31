using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um serviço cadastrado para uso no catálogo da aplicação.
/// </summary>
public class Servico : Entity
{
    /// <summary>
    /// Nome do serviço.
    /// </summary>
    public string Nome { get; private set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada do serviço.
    /// </summary>
    public string? Descricao { get; private set; }

    /// <summary>
    /// Valor cobrado pelo serviço.
    /// </summary>
    public decimal Valor { get; private set; }

    private Servico()
    {
    }

    /// <summary>
    /// Cria um novo serviço.
    /// </summary>
    /// <param name="nome">Nome do serviço.</param>
    /// <param name="descricao">Descrição do serviço.</param>
    /// <param name="valor">Valor cobrado.</param>
    public Servico(
        string nome,
        string? descricao,
        decimal valor)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do serviço é obrigatório.");

        if (valor <= 0)
            throw new DomainException("O valor do serviço deve ser maior que zero.");

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        Valor = valor;
    }

    /// <summary>
    /// Atualiza os dados cadastrais do serviço.
    /// </summary>
    /// <param name="nome">Novo nome do serviço.</param>
    /// <param name="descricao">Nova descrição do serviço.</param>
    /// <param name="valor">Novo valor do serviço.</param>
    public void AtualizarDados(string nome, string? descricao, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do serviço é obrigatório.");

        if (valor <= 0)
            throw new DomainException("O valor do serviço deve ser maior que zero.");

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        Valor = valor;

        AtualizarDataModificacao();
    }

    /// <summary>
    /// Ativa o serviço.
    /// </summary>
    public void Ativar()
    {
        Reativar();
    }

    /// <summary>
    /// Desativa o serviço.
    /// </summary>
    public void Desativar()
    {
        if (EstaExcluida())
            return;

        Excluir();
    }

}