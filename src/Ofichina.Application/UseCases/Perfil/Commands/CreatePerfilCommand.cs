using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Perfis.Commands;

/// <summary>
/// Comando para criar um novo perfil.
/// </summary>
public class CreatePerfilCommand : ICommand<Result>
{
    public string NomePerfil { get; set; }

    public string Descricao { get; set; }

    public CreatePerfilCommand(string nomePerfil, string descricao)
    {
        NomePerfil = nomePerfil;
        Descricao = descricao;
    }
}