using Ofichina.Authentication.Validators;
using Ofichina.Contracts.Requests.Autenticacao;

namespace Ofichina.UnitTests.Authentication;

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

    [Fact]
    public void Deve_Rejeitar_Email_Vazio()
    {
        var validator = new AutenticacaoRequestValidator();
        var command = new AutenticacaoRequest { Email = string.Empty, Senha = "123456" };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(AutenticacaoRequest.Email));
    }

    [Fact]
    public void Deve_Rejeitar_Email_Muito_Longo()
    {
        var validator = new AutenticacaoRequestValidator();
        var command = new AutenticacaoRequest
        {
            Email = new string('a', 201) + "@ofichinna.com",
            Senha = "123456"
        };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(AutenticacaoRequest.Email));
    }

    [Fact]
    public void Deve_Rejeitar_Senha_Vazia()
    {
        var validator = new AutenticacaoRequestValidator();
        var command = new AutenticacaoRequest { Email = "maria@ofichinna.com", Senha = string.Empty };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(AutenticacaoRequest.Senha));
    }

    [Fact]
    public void Deve_Rejeitar_Senha_Curta()
    {
        var validator = new AutenticacaoRequestValidator();
        var command = new AutenticacaoRequest { Email = "maria@ofichinna.com", Senha = "123" };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(AutenticacaoRequest.Senha));
    }
}