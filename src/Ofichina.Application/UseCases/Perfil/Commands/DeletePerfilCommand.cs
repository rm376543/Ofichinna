using Ofichina.Application.Abstractions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Perfil.Commands;

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