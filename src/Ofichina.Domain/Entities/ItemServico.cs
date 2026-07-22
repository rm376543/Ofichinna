using System.ComponentModel.DataAnnotations.Schema;
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
    /// Navegação para a peça de serviço vinculada ao item.
    /// </summary>
    public PecaServico? PecaServico { get; private set; }

    /// <summary>
    /// Identificador da peça de serviço vinculada ao item.
    /// </summary>
    public Guid PecaServicoId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Identificador do vínculo da peça de serviço.
    /// </summary>
    [NotMapped]
    public Guid ServicoId => PecaServicoId;

    /// <summary>
    /// Identificador da ordem de serviço à qual o serviço pertence.
    /// </summary>
    public Guid OrdemServicoId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Descrição da peça de serviço vinculada.
    /// </summary>
    [NotMapped]
    public string Descricao => PecaServico?.Peca?.Nome ?? string.Empty;


    /// <summary>
    /// Valor da peça de serviço vinculada.
    /// </summary>
    [NotMapped]
    public decimal Valor => PecaServico?.Peca?.Valor ?? 0;


    /// <summary>
    /// Peça de serviço vinculada ao item.
    /// </summary>
    [NotMapped]
    public IReadOnlyCollection<PecaServico> Pecas => PecaServico is null ? [] : [PecaServico];


    /// <summary>
    /// Valor total da peça de serviço vinculada.
    /// </summary>
    [NotMapped]
    public decimal ValorTotal => 
        PecaServico?.ValorTotal ?? 0;


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
    internal ItemServico(
        Guid ordemServicoId,
        Guid pecaServicoId)
    {
        if (ordemServicoId == Guid.Empty)
            throw new DomainException(
                "Ordem de serviço obrigatória.");

        if (pecaServicoId == Guid.Empty)
            throw new DomainException("Peça de serviço obrigatória.");

        PecaServicoId = pecaServicoId;
        OrdemServicoId = ordemServicoId;
    }


    /// <summary>
    /// Cria um item de serviço vinculado a uma ordem de serviço.
    /// </summary>
    public static ItemServico Criar(
        Guid ordemServicoId,
        Guid pecaServicoId)
    {
        return new ItemServico(ordemServicoId, pecaServicoId);
    }


    /// <summary>
    /// Atualiza os dados do serviço vinculado à ordem.
    /// </summary>
    public void AtualizarDados(Guid pecaServicoId)
    {
        ValidarNaoExcluido();

        if (pecaServicoId == Guid.Empty)
            throw new DomainException("Peça de serviço obrigatória.");

        PecaServicoId = pecaServicoId;

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Atualiza o vínculo e os dados do serviço catalogado.
    /// </summary>
    public void AtualizarServico(Guid pecaServicoId)
    {
        if (pecaServicoId == Guid.Empty)
            throw new DomainException("Peça de serviço obrigatória.");

        ValidarNaoExcluido();

        PecaServicoId = pecaServicoId;
        AtualizarDataModificacao();
    }


    /// <summary>
    /// Adiciona uma peça ao serviço associado ao item.
    /// </summary>
    public void AdicionarPeca(
        Guid pecaId,
        int quantidade)
    {
        if (PecaServico is null)
            throw new DomainException("Peça de serviço não encontrada.");

        PecaServico.AtualizarDados(pecaId, quantidade);
    }


    /// <summary>
    /// Atualiza uma peça vinculada ao serviço associado ao item.
    /// </summary>
    public void AtualizarPeca(
        Guid pecaServicoId,
        Guid pecaId,
        int quantidade)
    {
        if (PecaServico is null)
            throw new DomainException("Peça de serviço não encontrada.");

        PecaServico.AtualizarDados(pecaId, quantidade);
    }


    /// <summary>
    /// Remove uma peça do serviço associado ao item.
    /// </summary>
    public void RemoverPeca(Guid pecaServicoId)
    {
        if (PecaServico is null)
            throw new DomainException("Peça de serviço não encontrada.");

        if (PecaServicoId != pecaServicoId)
            throw new DomainException("Peça de serviço não encontrada.");

        Excluir();
    }


    /// <summary>
    /// Marca uma peça como utilizada.
    /// </summary>
    public void UtilizarPeca(Guid pecaServicoId)
    {
        if (PecaServico is null)
            throw new DomainException("Peça de serviço não encontrada.");

        if (PecaServicoId != pecaServicoId)
            throw new DomainException("Peça de serviço não encontrada.");

        PecaServico.MarcarComoUtilizada();
    }


    /// <summary>
    /// Obtém uma peça vinculada ao serviço associado ao item.
    /// </summary>
    public PecaServico? ObterPeca(Guid pecaServicoId)
    {
        return PecaServico is not null && PecaServicoId == pecaServicoId ? PecaServico : null;
    }


    private void ValidarNaoExcluido()
    {
        if (EstaExcluida())
            throw new DomainException("Não é possível alterar um item de serviço removido.");
    }
}