using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Authentication;

namespace Ofichina.Application.UseCases.Autenticacao.Commands;

public sealed class AutenticarCommand : ICommand<Result<AuthenticationResponse>>
{
    public string Email { get; init; }
    public string Senha { get; init; }

    public AutenticarCommand(string email, string senha)
    {
        Email = email;
        Senha = senha;
    }
}