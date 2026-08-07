using Ofichina.Application.Validators.Servicos;
using Ofichina.Contracts.Requests.Servicos;

namespace Ofichina.UnitTests.Application.Servicos;

public sealed class ServicoRequestValidatorTests
{
    [Fact]
    public void CreateValidator_Deve_Aceitar_Dados_Validos()
    {
        var validator = new CreateServicoRequestValidator();
        var request = new CreateServicoRequest
        {
            Nome = "Troca de óleo",
            Descricao = "Serviço completo",
            Valor = 149.90m,
            Ativo = true
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateValidator_Deve_Rejeitar_Dados_Invalidos()
    {
        var validator = new CreateServicoRequestValidator();
        var request = new CreateServicoRequest
        {
            Nome = "",
            Descricao = new string('x', 501),
            Valor = 0m
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateServicoRequest.Nome));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateServicoRequest.Descricao));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateServicoRequest.Valor));
    }

    [Fact]
    public void UpdateValidator_Deve_Rejeitar_Id_Em_Branco()
    {
        var validator = new UpdateServicoRequestValidator();
        var request = new UpdateServicoRequest
        {
            Nome = "Troca de óleo",
            Descricao = "Serviço completo",
            Valor = 149.90m
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateServicoRequest.ServicoId));
    }
}