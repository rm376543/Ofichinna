using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;

namespace Ofichina.Application.UseCases.Autenticacao.Commands;

public sealed class CadastrarClienteCommand : ICommand<Result<AutenticacaoResponse>>
{
    public string Nome { get; }
    public string Email { get; }
    public string Senha { get; }

    public CadastrarClienteCommand(string nome, string email, string senha)
    {
        Nome = nome;
        Email = email;
        Senha = senha;
    }
}