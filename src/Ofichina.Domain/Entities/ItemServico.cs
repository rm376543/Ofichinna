using Ofichina.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um serviço executado ou previsto dentro de uma ordem de serviço.
/// Esta entidade pertence ao agregado OrdemServico e seu ciclo de vida
/// é controlado pela própria ordem de serviço.
/// </summary>
public class ItemServico : Entity
{
    private readonly List<ServicoPeca> _pecas = [];

    /// <summary>
    /// Identificador da ordem de serviço à qual o serviço pertence.
    /// </summary>
    public Guid OrdemServicoId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Peças vinculadas ao item de serviço.
    /// </summary>
    public IReadOnlyCollection<ServicoPeca> Pecas => _pecas.Where(p => !p.EstaExcluida()).ToList().AsReadOnly();

    /// <summary>
    /// Identificador do serviço vinculado derivado da primeira peça ativa.
    /// </summary>
    [NotMapped]
    public Guid ServicoId => _pecas.FirstOrDefault(p => !p.EstaExcluida())?.ServicoId ?? Guid.Empty;

    /// <summary>
    /// Primeira peça ativa do item.
    /// </summary>
    [NotMapped]
    public ServicoPeca? ServicoPeca => _pecas.FirstOrDefault(p => !p.EstaExcluida());

    /// <summary>
    /// Identificador da primeira peça ativa.
    /// </summary>
    public Guid ServicoPecaId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Descrição derivada da primeira peça ativa ou vazia se não houver peças.
    /// </summary>
    [NotMapped]
    public string Descricao => _pecas.FirstOrDefault(p => !p.EstaExcluida())?.Peca?.Nome ?? string.Empty;

    /// <summary>
    /// Valor derivado da primeira peça ativa ou zero se não houver peças.
    /// </summary>
    [NotMapped]
    public decimal Valor => _pecas.FirstOrDefault(p => !p.EstaExcluida())?.Peca?.Valor ?? 0;

    /// <summary>
    /// Valor total calculado como soma de todas as peças ativas.
    /// </summary>
    [NotMapped]
    public decimal ValorTotal => _pecas.Where(p => !p.EstaExcluida()).Sum(p => p.ValorTotal);

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
    public ItemServico(Guid ordemServicoId)
    {
        if (ordemServicoId == Guid.Empty)
            throw new DomainException(
                "Ordem de serviço obrigatória.");

        OrdemServicoId = ordemServicoId;
    }

    /// <summary>
    /// Cria um item de serviço vinculado a uma ordem de serviço.
    /// </summary>
    public static ItemServico Criar(Guid ordemServicoId)
    {
        return new ItemServico(ordemServicoId);
    }

    /// <summary>
    /// Atualiza o item com a referência da primeira peça ativa.
    /// </summary>
    public void AtualizarServico(Guid servicoPecaId)
    {
        var peca = ObterPeca(servicoPecaId);

        if (peca is null)
            throw new DomainException("Peça não encontrada.");

        if (peca.EstaExcluida())
            throw new DomainException("Peça não encontrada.");

        ServicoPecaId = servicoPecaId;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Adiciona uma peça ao item de serviço.
    /// </summary>
    public void AdicionarPeca(
        ServicoPeca servicoPeca,
        int quantidade)
    {
        ValidarNaoExcluido();

        if (servicoPeca is null)
            throw new DomainException("Peça obrigatória.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade inválida.");

        if (_pecas.Any(p => p.PecaId == servicoPeca.PecaId && !p.EstaExcluida()))
            throw new DomainException("A peça já foi adicionada ao item de serviço.");

        var novaPeca = ServicoPeca.Criar(servicoPeca.ServicoId, servicoPeca.PecaId, quantidade);
        _pecas.Add(novaPeca);

        if (ServicoPecaId == Guid.Empty)
            ServicoPecaId = novaPeca.Id;

        AtualizarDataModificacao();
    }

    /// <summary>
    /// Adiciona uma peça ao item de serviço.
    /// </summary>
    public void AdicionarPeca(
        Guid pecaId,
        int quantidade)
    {
        throw new DomainException("É necessário informar a peça de serviço completa para adicionar ao item.");
    }

    /// <summary>
    /// Atualiza uma peça vinculada ao item de serviço.
    /// </summary>
    public void AtualizarPeca(
        Guid servicoPecaId,
        Guid pecaId,
        int quantidade)
    {
        ValidarNaoExcluido();

        var peca = _pecas.FirstOrDefault(p => p.Id == servicoPecaId);

        if (peca is null || peca.EstaExcluida())
            throw new DomainException("Peça não encontrada.");

        peca.AtualizarDados(pecaId, quantidade);

        if (ServicoPecaId == servicoPecaId)
            ServicoPecaId = peca.Id;

        AtualizarDataModificacao();
    }

    /// <summary>
    /// Substitui todas as peças ativas do item.
    /// </summary>
    public void SubstituirPecas(IEnumerable<ServicoPeca> novasPecas)
    {
        ValidarNaoExcluido();

        var pecasAtivas = _pecas.Where(p => !p.EstaExcluida()).ToList();

        foreach (var peca in pecasAtivas)
        {
            peca.ValidarRemocao();
            peca.Excluir();
        }

        foreach (var peca in novasPecas)
        {
            if (peca.EstaExcluida())
                throw new DomainException("Peça não encontrada.");

            _pecas.Add(ServicoPeca.Criar(peca.ServicoId, peca.PecaId, peca.Quantidade));
        }

        ServicoPecaId = _pecas.FirstOrDefault(p => !p.EstaExcluida())?.Id ?? Guid.Empty;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Remove uma peça do item de serviço.
    /// </summary>
    public void RemoverPeca(Guid servicoPecaId)
    {
        ValidarNaoExcluido();

        var peca = _pecas.FirstOrDefault(p => p.Id == servicoPecaId);

        if (peca is null || peca.EstaExcluida())
            throw new DomainException("Peça não encontrada.");

        peca.Excluir();

        if (ServicoPecaId == servicoPecaId)
            ServicoPecaId = _pecas.FirstOrDefault(p => !p.EstaExcluida())?.Id ?? Guid.Empty;

        AtualizarDataModificacao();
    }

    /// <summary>
    /// Marca uma peça como utilizada.
    /// </summary>
    public void UtilizarPeca(Guid servicoPecaId)
    {
        ValidarNaoExcluido();

        var peca = _pecas.FirstOrDefault(p => p.Id == servicoPecaId);

        if (peca is null || peca.EstaExcluida())
            throw new DomainException("Peça não encontrada.");

        peca.MarcarComoUtilizada();
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Obtém uma peça vinculada ao item de serviço.
    /// </summary>
    public ServicoPeca? ObterPeca(Guid servicoPecaId)
    {
        return _pecas.FirstOrDefault(p => p.Id == servicoPecaId && !p.EstaExcluida());
    }

    private void ValidarNaoExcluido()
    {
        if (EstaExcluida())
            throw new DomainException("Não é possível alterar um item de serviço removido.");
    }
}