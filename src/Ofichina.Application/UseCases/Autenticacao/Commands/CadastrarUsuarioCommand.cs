using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Authentication;

namespace Ofichina.Application.UseCases.Autenticacao.Commands;

public sealed class CadastrarUsuarioCommand : ICommand<Result<AuthenticationResponse>>
{
    public string Email { get; }
    public string Senha { get; }

    public CadastrarUsuarioCommand(string email, string senha)
    {
        Email = email;
        Senha = senha;
    }
}