using Microsoft.Extensions.Configuration;
using Ofichina.Authentication.Services;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Ofichina.UnitTests.Authentication;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public async Task GerarTokenAsync_Deve_Lancar_Excecao_Quando_Chave_Nao_EstiverConfigurada()
    {
        var service = new JwtTokenService(ConstruirConfiguracao());
        var usuario = new Usuario(new Email("maria@ofichinna.com"), "hash:123456");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GerarTokenAsync(usuario, ["ADMIN"]));

        Assert.Equal("Jwt:Key não configurada.", exception.Message);
    }

    [Fact]
    public async Task GerarTokenAsync_Deve_Usar_Valores_Padrao_Quando_Configuracao_Nao_For_Informada()
    {
        var service = new JwtTokenService(ConstruirConfiguracao(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "chave-super-secreta-para-testes-1234567890"
        }));
        var usuario = new Usuario(new Email("maria@ofichinna.com"), "hash:123456");

        var response = await service.GerarTokenAsync(usuario, []);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);

        Assert.Equal("ofichinna", token.Issuer);
        Assert.Single(token.Audiences);
        Assert.Contains("ofichinna", token.Audiences);
        Assert.Equal(usuario.Email.Value, token.Claims.First(x => x.Type == JwtRegisteredClaimNames.UniqueName).Value);
        Assert.Equal(usuario.Email.Value, token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal(usuario.Id.ToString(), token.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
        Assert.InRange((response.ExpiraEm - DateTime.UtcNow).TotalMinutes, 59, 61);
    }

    [Fact]
    public async Task GerarTokenAsync_Deve_Gerar_Claims_E_Perfis_Informados()
    {
        var service = new JwtTokenService(ConstruirConfiguracao(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "issuer-test",
            ["Jwt:Audience"] = "audience-test",
            ["Jwt:Key"] = "chave-super-secreta-para-testes-1234567890",
            ["Jwt:ExpirationMinutes"] = "15"
        }));

        var usuario = new Usuario(new Email("maria@ofichinna.com"), "hash:123456");

        var response = await service.GerarTokenAsync(usuario, ["ADMIN", "GERENTE"]);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);

        Assert.Equal("issuer-test", token.Issuer);
        Assert.Equal(["audience-test"], token.Audiences);
        Assert.Equal(usuario.Id.ToString(), token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(usuario.Email.Value, token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal(2, token.Claims.Count(x => x.Type == ClaimTypes.Role));
        Assert.Contains(token.Claims, x => x.Type == ClaimTypes.Role && x.Value == "ADMIN");
        Assert.Contains(token.Claims, x => x.Type == ClaimTypes.Role && x.Value == "GERENTE");
        Assert.InRange((response.ExpiraEm - DateTime.UtcNow).TotalMinutes, 14, 16);
    }

    private static IConfiguration ConstruirConfiguracao(Dictionary<string, string?>? valores = null)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(valores ?? [])
            .Build();
    }
}