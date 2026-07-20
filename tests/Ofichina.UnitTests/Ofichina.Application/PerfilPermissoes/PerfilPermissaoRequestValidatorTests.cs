using Ofichina.Application.Validators.PerfilPermissao;
using Ofichina.Contracts.Requests.PerfilPermissao;

namespace Ofichina.UnitTests.Application.PerfisPermissoes;

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
