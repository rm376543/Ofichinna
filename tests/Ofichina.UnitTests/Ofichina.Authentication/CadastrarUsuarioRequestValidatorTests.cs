using Ofichina.Authentication.Validators;
using Ofichina.Contracts.Requests.Usuario;

namespace Ofichina.UnitTests.Autenticacao;

public sealed class CadastrarUsuarioRequestValidatorTests
{
    [Fact]
    public void Deve_Validar_Usuario_Valido()
    {
        var validator = new CadastrarUsuarioRequestValidator();
        var command = new CadastrarUsuarioRequest { Email = "maria@ofichinna.com", Senha = "123456" };

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Rejeitar_Email_Invalido()
    {
        var validator = new CadastrarUsuarioRequestValidator();
        var command = new CadastrarUsuarioRequest { Email = "email-invalido", Senha = "123456" };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CadastrarUsuarioRequest.Email));
    }

    [Fact]
    public void Deve_Rejeitar_Email_Vazio()
    {
        var validator = new CadastrarUsuarioRequestValidator();
        var command = new CadastrarUsuarioRequest { Email = string.Empty, Senha = "123456" };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CadastrarUsuarioRequest.Email));
    }

    [Fact]
    public void Deve_Rejeitar_Email_Muito_Longo()
    {
        var validator = new CadastrarUsuarioRequestValidator();
        var command = new CadastrarUsuarioRequest
        {
            Email = new string('a', 201) + "@ofichinna.com",
            Senha = "123456"
        };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CadastrarUsuarioRequest.Email));
    }

    [Fact]
    public void Deve_Rejeitar_Senha_Vazia()
    {
        var validator = new CadastrarUsuarioRequestValidator();
        var command = new CadastrarUsuarioRequest { Email = "maria@ofichinna.com", Senha = string.Empty };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CadastrarUsuarioRequest.Senha));
    }

    [Fact]
    public void Deve_Rejeitar_Senha_Curta()
    {
        var validator = new CadastrarUsuarioRequestValidator();
        var command = new CadastrarUsuarioRequest { Email = "maria@ofichinna.com", Senha = "123" };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CadastrarUsuarioRequest.Senha));
    }
}