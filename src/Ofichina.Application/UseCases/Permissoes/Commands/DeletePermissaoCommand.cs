using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Permissoes.Commands;

public sealed class DeletePermissaoCommand : ICommand<Result>
{
    public Guid Id { get; }

    public DeletePermissaoCommand(Guid id)
    {
        Id = id;
    }
}
