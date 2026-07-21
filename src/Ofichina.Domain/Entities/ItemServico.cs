using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um serviço executado ou previsto dentro de uma ordem de serviço.
/// Esta entidade pertence ao agregado OrdemServico e seu ciclo de vida
/// é controlado pela própria ordem de serviço.
/// </summary>
public class ItemServico : Entity
{
    private readonly List<PecaServico> _pecas = [];

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
    /// Peças adicionadas ao serviço.
    /// </summary>
    public IReadOnlyCollection<PecaServico> Pecas => _pecas.AsReadOnly();


    /// <summary>
    /// Valor total do serviço.
    /// Corresponde ao valor do serviço mais o valor total das peças não excluídas.
    /// </summary>
    public decimal ValorTotal => 
        Valor + _pecas.Where(x => !x.EstaExcluida()).Sum(x => x.ValorTotal);


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
        ValidarNaoExcluido();

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


    /// <summary>
    /// Atualiza o vínculo e os dados do serviço catalogado.
    /// </summary>
    public void AtualizarServico(
        Guid servicoId,
        string descricao,
        decimal valor)
    {
        if (servicoId == Guid.Empty)
            throw new DomainException("Serviço obrigatório.");

        ValidarNaoExcluido();

        ServicoId = servicoId;
        AtualizarDados(descricao, valor);
    }


    /// <summary>
    /// Adiciona uma peça ao serviço.
    /// </summary>
    public void AdicionarPeca(
        Guid pecaId,
        string descricao,
        int quantidade,
        decimal valorUnitario)
    {
        ValidarNaoExcluido();

        if (_pecas.Any(x => x.PecaId == pecaId && !x.EstaExcluida()))
            throw new DomainException("A peça já foi adicionada ao serviço.");

        var item = new PecaServico(
            Id,
            pecaId,
            descricao,
            quantidade,
            valorUnitario);

        _pecas.Add(item);

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Atualiza uma peça vinculada ao serviço.
    /// </summary>
    public void AtualizarPeca(
        Guid pecaServicoId,
        Guid pecaId,
        string descricao,
        int quantidade,
        decimal valorUnitario)
    {
        var peca = _pecas.FirstOrDefault(x => x.Id == pecaServicoId);

        if (peca is null)
            throw new DomainException("Peça não encontrada.");

        if (peca.EstaExcluida())
            throw new DomainException("Peça não encontrada.");

        peca.AtualizarDados(pecaId, descricao, quantidade, valorUnitario);

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Remove uma peça do serviço.
    /// </summary>
    public void RemoverPeca(Guid pecaServicoId)
    {
        var peca = _pecas.FirstOrDefault(x => x.Id == pecaServicoId);

        if (peca is null)
            throw new DomainException("Peça não encontrada.");

        if (peca.EstaExcluida())
            throw new DomainException("Peça não encontrada.");

        peca.ValidarRemocao();
        peca.Excluir();

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Marca uma peça como utilizada.
    /// </summary>
    public void UtilizarPeca(Guid pecaServicoId)
    {
        var peca = _pecas.FirstOrDefault(x => x.Id == pecaServicoId);

        if (peca is null)
            throw new DomainException("Peça não encontrada.");

        if (peca.EstaExcluida())
            throw new DomainException("Não é possível utilizar uma peça removida.");

        peca.MarcarComoUtilizada();

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Obtém uma peça pelo identificador.
    /// </summary>
    public PecaServico? ObterPeca(Guid pecaServicoId)
    {
        return _pecas.FirstOrDefault(x => x.Id == pecaServicoId);
    }


    private void ValidarNaoExcluido()
    {
        if (EstaExcluida())
            throw new DomainException("Não é possível alterar um item de serviço removido.");
    }
}