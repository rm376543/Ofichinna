using Microsoft.AspNetCore.Http;
using Ofichina.Authentication.Security;
using Ofichina.Authentication.Services;
using Ofichina.Application.Abstractions.Authentication.Service;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Ofichina.UnitTests.Authentication;

public sealed class SecurityUtilitiesTests
{
    [Fact]
    public void PasswordHasher_Deve_Gerar_E_Verificar_Hash()
    {
        var hash = PasswordHasher.Hash("Senha@123");

        Assert.True(PasswordHasher.Verify("Senha@123", hash));
    }

    [Fact]
    public void PasswordHasher_Deve_Gerar_Hash_Deterministico_Com_Salt_Informado()
    {
        var salt = Enumerable.Range(1, 16).Select(x => (byte)x).ToArray();

        var hash = PasswordHasher.Hash("Senha@123", salt);

        Assert.StartsWith("100000.", hash);
        Assert.True(PasswordHasher.Verify("Senha@123", hash));
    }

    [Fact]
    public void PasswordHasher_Deve_Rejeitar_Formato_Invalido()
    {
        Assert.False(PasswordHasher.Verify("Senha@123", "hash-invalido"));
    }

    [Fact]
    public void PasswordHasher_Deve_Rejeitar_Senha_Diferente()
    {
        var hash = PasswordHasher.Hash("Senha@123");

        Assert.False(PasswordHasher.Verify("OutraSenha@123", hash));
    }

    [Fact]
    public void SenhaHasherService_Deve_Delegar_Para_Utilitario_Estatico()
    {
        IPasswordHasherService service = new SenhaHasherService();

        var hash = service.GerarHash("Senha@123");

        Assert.True(service.Verificar("Senha@123", hash));
    }

    [Fact]
    public void UsuarioAtualService_Deve_Retornar_Null_Quando_Nao_Houver_Contexto()
    {
        var service = new UsuarioAtualService(new HttpContextAccessor());

        Assert.Null(service.ObterUsuarioId());
    }

    [Fact]
    public void UsuarioAtualService_Deve_Usar_NameIdentifier_Quando_Presente()
    {
        var usuarioId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = CriarContexto(new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()))
        };

        var service = new UsuarioAtualService(httpContextAccessor);

        Assert.Equal(usuarioId, service.ObterUsuarioId());
    }

    [Fact]
    public void UsuarioAtualService_Deve_Usar_Sub_Quando_NameIdentifier_Nao_Existir()
    {
        var usuarioId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = CriarContexto(new Claim(global::System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, usuarioId.ToString()))
        };

        var service = new UsuarioAtualService(httpContextAccessor);

        Assert.Equal(usuarioId, service.ObterUsuarioId());
    }

    [Fact]
    public void UsuarioAtualService_Deve_Retornar_Null_Quando_Claim_For_Invalida()
    {
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = CriarContexto(new Claim(ClaimTypes.NameIdentifier, "nao-e-guid"))
        };

        var service = new UsuarioAtualService(httpContextAccessor);

        Assert.Null(service.ObterUsuarioId());
    }

    private static DefaultHttpContext CriarContexto(params Claim[] claims)
    {
        var contexto = new DefaultHttpContext();
        contexto.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        return contexto;
    }
}