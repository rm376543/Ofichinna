using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Application.Validators;

namespace Ofichina.UnitTests.Autenticacao;

public sealed class CadastrarUsuarioCommandValidatorTests
{
    [Fact]
    public void Deve_Validar_Comando_Valido()
    {
        var validator = new CadastrarUsuarioCommandValidator();
        var command = new CadastrarUsuarioCommand("Maria Silva", "maria@ofichinna.com", "123456");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Rejeitar_Email_Invalido_E_Senha_Curta()
    {
        var validator = new CadastrarUsuarioCommandValidator();
        var command = new CadastrarUsuarioCommand("Maria Silva", "email-invalido", "123");

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CadastrarUsuarioCommand.Email));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CadastrarUsuarioCommand.Senha));
    }
}