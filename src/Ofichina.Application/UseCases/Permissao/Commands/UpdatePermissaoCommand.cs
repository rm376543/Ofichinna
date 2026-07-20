using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Permissoes.Commands;

public sealed class UpdatePermissaoCommand : ICommand<Result>
{
    public Guid Id { get; }

    public string Codigo { get; }

    public string Descricao { get; }

    public UpdatePermissaoCommand(Guid id, string codigo, string descricao)
    {
        Id = id;
        Codigo = codigo;
        Descricao = descricao;
    }
}
