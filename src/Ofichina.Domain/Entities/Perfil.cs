namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um perfil de autorização da aplicação.
/// </summary>
public class Perfil : Entity
{
    /// <summary>
    /// Código único do perfil, usado como role/claim.
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nome descritivo do perfil.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do perfil.
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Indica se o perfil está disponível para uso.
    /// </summary>
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Usuários vinculados ao perfil.
    /// </summary>
    public ICollection<UsuarioPerfil> Usuarios { get; set; } = [];

    private Perfil()
    {
    }

    public Perfil(string codigo, string nome, string? descricao = null)
    {
        Codigo = codigo;
        Nome = nome;
        Descricao = descricao;
    }
}