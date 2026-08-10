using Ofichina.Application.Validators.Orcamento;
using Ofichina.Contracts.Requests.Orcamento;

namespace Ofichina.UnitTests.Application.Orcamentos;

public sealed class UpdateOrcamentoDescontoRequestValidatorTests
{
    [Fact]
    public void Deve_Aceitar_Desconto_Valido()
    {
        var validator = new UpdateOrcamentoDescontoRequestValidator();

        var result = validator.Validate(new UpdateOrcamentoDescontoRequest
        {
            Desconto = 10m,
            DescontoEmDinheiro = false
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Aceitar_Desconto_Percentual_Valido()
    {
        var validator = new UpdateOrcamentoDescontoRequestValidator();

        var result = validator.Validate(new UpdateOrcamentoDescontoRequest
        {
            Desconto = 15m,
            DescontoEmDinheiro = true
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Rejeitar_Desconto_Negativo()
    {
        var validator = new UpdateOrcamentoDescontoRequestValidator();

        var result = validator.Validate(new UpdateOrcamentoDescontoRequest
        {
            Desconto = -1m,
            DescontoEmDinheiro = false
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrcamentoDescontoRequest.Desconto));
    }

    [Fact]
    public void Deve_Rejeitar_Desconto_Percentual_Maior_Que_Cem()
    {
        var validator = new UpdateOrcamentoDescontoRequestValidator();

        var result = validator.Validate(new UpdateOrcamentoDescontoRequest
        {
            Desconto = 101m,
            DescontoEmDinheiro = true
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrcamentoDescontoRequest.Desconto));
    }
}