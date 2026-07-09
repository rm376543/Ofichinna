using Ofichina.Application.Abstractions;

namespace Ofichina.Application.UseCases.Perfil.Commands;

/// <summary>
/// Comando para criar um novo perfil.
/// </summary>
public class CreatePerfilCommand : ICommand<Guid>
{
    public string Codigo { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public bool Ativo { get; set; } = true;

    public CreatePerfilCommand(string codigo, string nome, string? descricao = null, bool ativo = true)
    {
        Codigo = codigo;
        Nome = nome;
        Descricao = descricao;
        Ativo = ativo;
    }
}