using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Permissoes.Commands;

public sealed class UpdatePermissaoCommand : ICommand<Result>
{
    public Guid PermissaoId { get; }

    public string Codigo { get; }

    public string Descricao { get; }

    public UpdatePermissaoCommand(Guid idPermissao, string codigo, string descricao)
    {
        PermissaoId = idPermissao;
        Codigo = codigo;
        Descricao = descricao;
    }
}
