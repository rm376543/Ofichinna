using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;

namespace Ofichina.Application.UseCases.Autenticacao.Commands;

public sealed class AutenticarCommand : ICommand<Result<AutenticacaoResponse>>
{
    public string Email { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;

    public AutenticarCommand(string email, string senha)
    {
        Email = email;
        Senha = senha;
    }
}