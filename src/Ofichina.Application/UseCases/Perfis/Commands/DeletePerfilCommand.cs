using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Perfis.Commands;

/// <summary>
/// Comando para desativar um perfil.
/// </summary>
public class DeletePerfilCommand : ICommand<Result>
{
    public Guid Id { get; set; }

    public DeletePerfilCommand(Guid id)
    {
        Id = id;
    }
}