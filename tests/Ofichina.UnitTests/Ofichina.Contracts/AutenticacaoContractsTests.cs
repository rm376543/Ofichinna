using Ofichina.Contracts.Requests.Autenticacao;
using Ofichina.Contracts.Responses;

namespace Ofichina.UnitTests.Contracts;

public class AutenticacaoResponseTests
{
    [Fact]
    public void AutenticacaoRequest_DeveManterOsValoresInformados()
    {
        var request = new AutenticacaoRequest
        {
            Email = "admin@ofichinna.com",
            Senha = "Senha@123"
        };

        Assert.Equal("admin@ofichinna.com", request.Email);
        Assert.Equal("Senha@123", request.Senha);
    }

}