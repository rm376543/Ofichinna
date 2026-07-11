using Ofichina.Authentication.Validators;
using Ofichina.Contracts.Requests.Autenticacao;

namespace Ofichina.UnitTests.Autenticacao;

public sealed class CadastrarUsuarioCommandValidatorTests
{
    [Fact]
    public void Deve_Validar_Comando_Valido()
    {
        var validator = new AutenticacaoRequestValidator();
        var command = new AutenticacaoRequest { Email = "maria@ofichinna.com", Senha = "123456" };

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Rejeitar_Email_Invalido_E_Senha_Curta()
    {
        var validator = new AutenticacaoRequestValidator();
        var command = new AutenticacaoRequest { Email = "email-invalido", Senha = "123" };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(AutenticacaoRequest.Email));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(AutenticacaoRequest.Senha));
    }
}