using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Entidade de exemplo para demonstrar a arquitetura.
/// Esta é uma entidade do domínio que herda de Entity.
/// </summary>
public class Exemplo : Entity
{
    /// <summary>
    /// Nome do exemplo.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do exemplo.
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Status do exemplo.
    /// </summary>
    public bool Ativo { get; set; } = true;

    private Exemplo() { }

    /// <summary>
    /// Cria uma nova instância do Exemplo.
    /// </summary>
    public Exemplo(string nome, string? descricao = null)
    {
        Nome = nome;
        Descricao = descricao;
    }
}
