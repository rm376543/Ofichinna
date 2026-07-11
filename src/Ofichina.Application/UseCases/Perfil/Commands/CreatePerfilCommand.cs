using Ofichina.Application.Abstractions;

namespace Ofichina.Application.UseCases.Perfis.Commands;

/// <summary>
/// Comando para criar um novo perfil.
/// </summary>
public class CreatePerfilCommand : ICommand<Guid>
{
    public string NomePerfil { get; set; } = string.Empty;

    public string Descricao { get; set; }

    public CreatePerfilCommand(string nomePerfil, string descricao)
    {
        NomePerfil = nomePerfil;
        Descricao = descricao;
    }
}