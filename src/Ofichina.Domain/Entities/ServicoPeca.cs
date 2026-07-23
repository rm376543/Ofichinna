using System.ComponentModel.DataAnnotations.Schema;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa uma peça vinculada a um serviço cadastrado.
/// </summary>
public class ServicoPeca : Entity
{
    /// <summary>
    /// Identificador da peça relacionada ao serviço.
    /// </summary>
    public Guid PecaId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Navegação para a peça cadastrada.
    /// </summary>
    public Peca? Peca { get; private set; }

    /// <summary>
    /// Identificador do serviço ao qual a peça pertence.
    /// </summary>
    public Guid ServicoId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Navegação para o serviço vinculado.
    /// </summary>
    public Servico? Servico { get; private set; }

    /// <summary>
    /// Identificador do item de serviço ao qual a peça pertence (quando vinculada a uma ordem).
    /// </summary>
    public Guid? ItemServicoId { get; private set; }

    /// <summary>
    /// Quantidade de peças utilizadas ou previstas na ordem de serviço.
    /// </summary>
    public int Quantidade { get; private set; } = 0;

    /// <summary>
    /// Descrição da peça vinculada, derivada do cadastro atual.
    /// </summary>
    [NotMapped]
    public string Descricao => Peca?.Nome ?? string.Empty;

    /// <summary>
    /// Valor unitário da peça, derivado do cadastro atual.
    /// </summary>
    [NotMapped]
    public decimal ValorUnitario => Peca?.Valor ?? 0;

    /// <summary>
    /// Valor total calculado da peça.
    /// </summary>
    [NotMapped]
    public decimal ValorTotal => Quantidade * ValorUnitario;

    /// <summary>
    /// Indica se a peça já foi aplicada/utilizada no veículo.
    /// </summary>
    public bool Utilizada { get; private set; }

    /// <summary>
    /// Data em que a peça foi marcada como utilizada.
    /// </summary>
    public DateTime? DataUtilizacao { get; private set; }

    private ServicoPeca()
    {
    }

    internal ServicoPeca(
        Guid servicoId,
        Guid pecaId,
        int quantidade)
    {
        if (servicoId == Guid.Empty)
            throw new DomainException("Serviço obrigatório.");

        if (pecaId == Guid.Empty)
            throw new DomainException("Peça obrigatória.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade inválida.");

        ServicoId = servicoId;
        PecaId = pecaId;
        Quantidade = quantidade;
    }

    /// <summary>
    /// Cria uma nova peça vinculada ao serviço.
    /// </summary>
    public static ServicoPeca Criar(
        Guid servicoId,
        Guid pecaId,
        int quantidade)
    {
        return new ServicoPeca(servicoId, pecaId, quantidade);
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
        int quantidade)
    {
        if (Utilizada)
            throw new DomainException("Não é possível alterar uma peça já utilizada.");

        if (pecaId == Guid.Empty)
            throw new DomainException("Peça obrigatória.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade inválida.");

        PecaId = pecaId;
        Quantidade = quantidade;

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
