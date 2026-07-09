namespace Ofichina.Contracts.Responses.Perfil;

/// <summary>
/// Resposta com os dados de um perfil.
/// </summary>
public class PerfilResponse
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public bool Ativo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}