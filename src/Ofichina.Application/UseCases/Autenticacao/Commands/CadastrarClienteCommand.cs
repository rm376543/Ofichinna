using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;

namespace Ofichina.Application.UseCases.Autenticacao.Commands;

public sealed class CadastrarClienteCommand : ICommand<Result<AutenticacaoResponse>>
{
    public string Email { get; }
    public string Senha { get; }

    public CadastrarClienteCommand(string email, string senha)
    {
        Email = email;
        Senha = senha;
    }
}