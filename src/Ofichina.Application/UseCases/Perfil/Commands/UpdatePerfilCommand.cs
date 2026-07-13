using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Perfis.Commands;

/// <summary>
/// Comando para atualizar um perfil.
/// </summary>
public class UpdatePerfilCommand : ICommand<Result>
{
    public Guid Id { get; set; }

    public string NomePerfil { get; set; }

    public string Descricao { get; set; }

    public UpdatePerfilCommand(Guid id, string nomePerfil, string descricao)
    {
        Id = id;
        NomePerfil = nomePerfil;
        Descricao = descricao;
    }
}