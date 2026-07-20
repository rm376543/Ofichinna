using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Permissoes.Commands;

public sealed class CreatePermissaoCommand : ICommand<Result<Guid>>
{
    public string Codigo { get; }

    public string Descricao { get; }

    public CreatePermissaoCommand(string codigo, string descricao)
    {
        Codigo = codigo;
        Descricao = descricao;
    }
}
