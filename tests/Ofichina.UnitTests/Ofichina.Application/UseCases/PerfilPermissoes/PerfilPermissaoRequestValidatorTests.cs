using Ofichina.Application.Validators.PerfilPermissoes;
using Ofichina.Contracts.Requests.PerfilPermissoes;

namespace Ofichina.UnitTests.Application.UseCases.PerfilPermissoes;

public sealed class PerfisPermissaoRequestValidatorTests
{
    [Fact]
    public void Validator_Deve_Rejeitar_Ids_Em_Branco()
    {
        var validator = new VincularPermissaoPerfilRequestValidator();
        var request = new VincularPermissaoPerfilRequest();

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VincularPermissaoPerfilRequest.PerfilId));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VincularPermissaoPerfilRequest.PermissaoId));
    }
}
