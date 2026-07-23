using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um serviço cadastrado para uso no catálogo da aplicação.
/// </summary>
public class Servico : Entity
{
    private readonly List<ServicoPeca> _pecas = [];

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

    /// <summary>
    /// Peças vinculadas ao serviço.
    /// </summary>
    public IReadOnlyCollection<ServicoPeca> Pecas => _pecas.AsReadOnly();

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

    /// <summary>
    /// Adiciona uma peça ao serviço.
    /// </summary>
    public void AdicionarPeca(
        Guid pecaId,
        int quantidade)
    {
        if (pecaId == Guid.Empty)
            throw new DomainException("Peça obrigatória.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade inválida.");

        if (_pecas.Any(x => x.PecaId == pecaId && !x.EstaExcluida()))
            throw new DomainException("A peça já foi adicionada ao serviço.");

        _pecas.Add(new ServicoPeca(Id, pecaId, quantidade));
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Atualiza uma peça vinculada ao serviço.
    /// </summary>
    public void AtualizarPeca(
        Guid pecaServicoId,
        Guid pecaId,
        int quantidade)
    {
        var peca = _pecas.FirstOrDefault(x => x.Id == pecaServicoId);

        if (peca is null || peca.EstaExcluida())
            throw new DomainException("Peça não encontrada.");

        peca.AtualizarDados(pecaId, quantidade);
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Remove uma peça vinculada ao serviço.
    /// </summary>
    public void RemoverPeca(Guid pecaServicoId)
    {
        var peca = _pecas.FirstOrDefault(x => x.Id == pecaServicoId);

        if (peca is null || peca.EstaExcluida())
            throw new DomainException("Peça não encontrada.");

        peca.ValidarRemocao();
        peca.Excluir();
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Remove todas as peças vinculadas ao serviço.
    /// </summary>
    public void RemoverTodasAsPecas()
    {
        var pecasAtivas = _pecas.Where(x => !x.EstaExcluida()).ToList();

        foreach (var peca in pecasAtivas)
        {
            peca.ValidarRemocao();
        }

        foreach (var peca in pecasAtivas)
        {
            peca.Excluir();
        }

        if (pecasAtivas.Count > 0)
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
    /// Obtém uma peça vinculada ao serviço.
    /// </summary>
    public ServicoPeca? ObterPeca(Guid pecaServicoId)
    {
        return _pecas.FirstOrDefault(x => x.Id == pecaServicoId);
    }

    /// <summary>
    /// Indica se o serviço possui peças ativas.
    /// </summary>
    public bool PossuiPecasAtivas()
    {
        return _pecas.Any(x => !x.EstaExcluida());
    }
}