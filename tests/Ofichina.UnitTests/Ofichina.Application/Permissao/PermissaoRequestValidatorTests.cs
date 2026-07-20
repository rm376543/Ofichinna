using Ofichina.Application.Validators.Permissao;
using Ofichina.Contracts.Requests.Permissao;

namespace Ofichina.UnitTests.Application.Permissao;

public sealed class PermissaoRequestValidatorTests
{
    [Fact]
    public void CreateValidator_Deve_Aceitar_Dados_Validos()
    {
        var validator = new CreatePermissaoRequestValidator();
        var request = new CreatePermissaoRequest
        {
            Codigo = "PERMISSAO_CADASTRAR",
            Descricao = "Permite cadastrar registros"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateValidator_Deve_Rejeitar_Dados_Invalidos()
    {
        var validator = new CreatePermissaoRequestValidator();
        var request = new CreatePermissaoRequest
        {
            Codigo = string.Empty,
            Descricao = string.Empty
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreatePermissaoRequest.Codigo));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreatePermissaoRequest.Descricao));
    }

    [Fact]
    public void UpdateValidator_Deve_Rejeitar_Id_Em_Branco()
    {
        var validator = new UpdatePermissaoRequestValidator();
        var request = new UpdatePermissaoRequest
        {
            Codigo = "PERMISSAO_ATUALIZADA",
            Descricao = "Permite atualizar registros"
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdatePermissaoRequest.Id));
    }
}
