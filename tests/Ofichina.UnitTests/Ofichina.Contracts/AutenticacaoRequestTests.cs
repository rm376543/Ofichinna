using Ofichina.Contracts.Requests.Autenticacao;
using Ofichina.Contracts.Responses.Authentication;

namespace Ofichina.UnitTests.Contracts;

public class AutenticacaoRequestTests
{
    [Fact]
    public void AutenticacaoResponse_DeveManterOsValoresInformados()
    {
        var expiraEm = new DateTime(2026, 07, 11, 12, 00, 00, DateTimeKind.Utc);

        var response = new AuthenticationResponse
        {
            UsuarioId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Email = "admin@ofichinna.com",
            Perfis = ["ADMIN"],
            AccessToken = "token-gerado",
            ExpiraEm = expiraEm
        };

        Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), response.UsuarioId);
        Assert.Equal("admin@ofichinna.com", response.Email);
        Assert.Equal(["ADMIN"], response.Perfis);
        Assert.Equal("token-gerado", response.AccessToken);
        Assert.Equal(expiraEm, response.ExpiraEm);
    }

    [Fact]
    public void AutenticacaoResponse_DeveHerdaDeTokenJwtResponse()
    {
        var response = new AuthenticationResponse();

        Assert.IsAssignableFrom<JwtResponse>(response);
    }
}