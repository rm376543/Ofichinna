using Ofichina.Application.Validators.Servicos;
using Ofichina.Contracts.Requests.Servicos;

namespace Ofichina.UnitTests.Application.Servicos.Pecas;

public sealed class CreateServicoPecaRequestValidatorTests
{
    [Fact]
    public void Deve_Aceitar_Dados_Validos()
    {
        var validator = new CreateServicoPecaRequestValidator();
        var request = new CreateServicoPecaRequest
        {
            ServicoId = Guid.NewGuid(),
            PecaId = Guid.NewGuid(),
            Quantidade = 2
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Rejeitar_Dados_Invalidos()
    {
        var validator = new CreateServicoPecaRequestValidator();
        var request = new CreateServicoPecaRequest();

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateServicoPecaRequest.ServicoId));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateServicoPecaRequest.PecaId));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateServicoPecaRequest.Quantidade));
    }
}