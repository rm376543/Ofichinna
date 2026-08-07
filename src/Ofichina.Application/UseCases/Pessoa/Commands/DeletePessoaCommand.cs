using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;

namespace Ofichina.Application.UseCases.Pessoas.Commands;

/// <summary>
/// Comando para desativar uma pessoa.
/// </summary>
public sealed class DeletePessoaCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da pessoa.
    /// </summary>
    public Guid Id { get; init; }

    public DeletePessoaCommand(RemovePessoaRequest request)
    {
        Id = request.Id;
    }
}
