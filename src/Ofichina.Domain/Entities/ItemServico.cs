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
    /// Identificador do serviço cadastrado vinculado ao item.
    /// </summary>
    public Guid ServicoId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Identificador da ordem de serviço à qual o serviço pertence.
    /// </summary>
    public Guid OrdemServicoId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Descrição do serviço no momento da inclusão na ordem de serviço.
    /// Mantém o histórico mesmo que o cadastro original seja alterado.
    /// </summary>
    public string Descricao { get; private set; } = string.Empty;


    /// <summary>
    /// Valor cobrado pelo serviço.
    /// </summary>
    public decimal Valor { get; private set; } = 0;


    /// <summary>
    /// Valor total do serviço.
    /// Para serviços sem quantidade, corresponde ao próprio valor informado.
    /// </summary>
    public decimal ValorTotal => Valor;


    /// <summary>
    /// Construtor utilizado pelo Entity Framework Core.
    /// </summary>
    private ItemServico()
    {
    }


    /// <summary>
    /// Cria um item de serviço vinculado a uma ordem de serviço.
    /// Este construtor possui acesso interno para garantir que a criação
    /// seja realizada somente pelo agregado OrdemServico.
    /// </summary>
    /// <param name="ordemServicoId">
    /// Identificador da ordem de serviço.
    /// </param>
    /// <param name="descricao">
    /// Descrição do serviço.
    /// </param>
    /// <param name="valor">
    /// Valor cobrado pelo serviço.
    /// </param>
    internal ItemServico(
        Guid servicoId,
        Guid ordemServicoId,
        string descricao,
        decimal valor)
    {
        if (servicoId == Guid.Empty)
            throw new DomainException(
                "Serviço obrigatório.");


        if (ordemServicoId == Guid.Empty)
            throw new DomainException(
                "Ordem de serviço obrigatória.");


        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException(
                "Descrição obrigatória.");


        if (valor <= 0)
            throw new DomainException(
                "Valor inválido.");


        ServicoId = servicoId;
        OrdemServicoId = ordemServicoId;
        Descricao = descricao.Trim();
        Valor = valor;
    }


    /// <summary>
    /// Atualiza os dados do serviço vinculado à ordem.
    /// </summary>
    /// <param name="descricao">Nova descrição do serviço.</param>
    /// <param name="valor">Novo valor do serviço.</param>
    public void AtualizarDados(
        string descricao,
        decimal valor)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException(
                "Descrição obrigatória.");


        if (valor <= 0)
            throw new DomainException(
                "Valor inválido.");


        Descricao = descricao.Trim();
        Valor = valor;

        AtualizarDataModificacao();
    }
}