using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using AuthenticationNamespace = Ofichina.Authentication;
using Ofichina.Authentication;
using Ofichina.Authentication.DependencyInjection;
using Ofichina.Authentication.Security;
using Ofichina.Authentication.Services;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Contracts.Common;

namespace Ofichina.UnitTests.Authentication;

public sealed class AuthorizationAndDependencyInjectionTests
{
    [Fact]
    public void PermissionRequirement_Deve_Remover_Espacos_Do_Direito_Acesso()
    {
        var requirement = new PermissionRequirement("  usuarios.listar  ");

        Assert.Equal("usuarios.listar", requirement.Permission);
    }

    [Fact]
    public async Task PermissionPolicyProvider_Deve_Criar_Policy_Com_Requirement()
    {
        var provider = new PermissionPolicyProvider(Options.Create(new AuthorizationOptions()));

        var policy = await provider.GetPolicyAsync("  usuarios.listar  ");

        Assert.NotNull(policy);
        var requirement = Assert.Single(policy!.Requirements.OfType<PermissionRequirement>());
        Assert.Equal("usuarios.listar", requirement.Permission);
    }

    [Fact]
    public async Task PermissionAuthorizationHandler_Deve_Aprovar_Quando_Usuario_Possuir_Permissao()
    {
        var usuarioId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var requirement = new PermissionRequirement("usuarios.listar");
        var user = CriarUsuarioPrincipal(usuarioId);
        var context = new AuthorizationHandlerContext([requirement], user, null);
        var handler = new PermissionAuthorizationHandler(new FakeProfileAuthService(true));

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task PermissionAuthorizationHandler_Deve_Ignorar_Quando_UsuarioId_For_Invalido()
    {
        var requirement = new PermissionRequirement("usuarios.listar");
        var user = CriarUsuarioPrincipal("nao-e-guid");
        var perfilService = new FakeProfileAuthService(true);
        var context = new AuthorizationHandlerContext([requirement], user, null);
        var handler = new PermissionAuthorizationHandler(perfilService);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
        Assert.Equal(0, perfilService.PermissaoCalls);
    }

    [Fact]
    public async Task PermissionAuthorizationHandler_Deve_Nao_Aprovar_Quando_Permissao_Nao_Existir()
    {
        var usuarioId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var requirement = new PermissionRequirement("usuarios.listar");
        var user = CriarUsuarioPrincipal(usuarioId);
        var perfilService = new FakeProfileAuthService(false);
        var context = new AuthorizationHandlerContext([requirement], user, null);
        var handler = new PermissionAuthorizationHandler(perfilService);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
        Assert.Equal(1, perfilService.PermissaoCalls);
    }

    [Fact]
    public async Task ApiAuthorizationMiddlewareResultHandler_Deve_Retornar_401_Quando_Challenge()
    {
        var handler = new ApiAuthorizationMiddlewareResultHandler();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await handler.HandleAsync(
            _ => Task.CompletedTask,
            context,
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(),
            PolicyAuthorizationResult.Challenge());

        context.Response.Body.Position = 0;
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.Contains("Usuário não autenticado.", body);
    }

    [Fact]
    public async Task ApiAuthorizationMiddlewareResultHandler_Deve_Retornar_403_Quando_Forbidden()
    {
        var handler = new ApiAuthorizationMiddlewareResultHandler();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await handler.HandleAsync(
            _ => Task.CompletedTask,
            context,
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(),
            PolicyAuthorizationResult.Forbid());

        context.Response.Body.Position = 0;
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.Contains("Você não tem permissão para acessar este recurso.", body);
    }

    [Fact]
    public async Task ApiAuthorizationMiddlewareResultHandler_Deve_Delegar_Quando_Houver_Sucesso()
    {
        var handler = new ApiAuthorizationMiddlewareResultHandler();
        var context = new DefaultHttpContext();
        var nextCalled = false;

        await handler.HandleAsync(
            _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            context,
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(),
            PolicyAuthorizationResult.Success());

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task LoggingJwtBearerEvents_Deve_Executar_Todos_Os_Metodos_Sem_Erro()
    {
        var events = new LoggingJwtBearerEvents(NullLogger<LoggingJwtBearerEvents>.Instance);
        var httpContext = new DefaultHttpContext();
        var scheme = new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, null, typeof(JwtBearerHandler));
        var options = new JwtBearerOptions();

        await events.MessageReceived(new MessageReceivedContext(httpContext, scheme, options));

        var failedContext = new AuthenticationFailedContext(httpContext, scheme, options)
        {
            Exception = new InvalidOperationException("falha")
        };
        await events.AuthenticationFailed(failedContext);

        await events.TokenValidated(new TokenValidatedContext(httpContext, scheme, options));
        await events.Challenge(new JwtBearerChallengeContext(httpContext, scheme, options, new AuthenticationProperties()));
    }

    [Fact]
    public void AddAuthenticationModules_Deve_Registrar_Servicos_E_Lancar_Sem_Key()
    {
        var services = new ServiceCollection();

        Assert.Throws<InvalidOperationException>(() => services.AddAuthenticationModules(ConstruirConfiguracao()));

        services.AddAuthenticationModules(ConstruirConfiguracao(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "chave-super-secreta-para-testes-1234567890"
        }));

        Assert.Contains(services, d => d.ServiceType == typeof(LoggingJwtBearerEvents));

        using var provider = services.BuildServiceProvider();
        var authOptions = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AuthenticationOptions>>().Value;
        var optionsMonitor = provider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<JwtBearerOptions>>();
        var options = optionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authOptions.DefaultAuthenticateScheme);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authOptions.DefaultChallengeScheme);
        Assert.Equal("ofichinna", options.TokenValidationParameters.ValidIssuer);
        Assert.Equal("ofichinna", options.TokenValidationParameters.ValidAudience);
        Assert.True(options.TokenValidationParameters.ValidateIssuerSigningKey);
        Assert.Equal(TimeSpan.Zero, options.TokenValidationParameters.ClockSkew);
        Assert.Equal(System.Security.Claims.ClaimTypes.NameIdentifier, options.TokenValidationParameters.NameClaimType);
        Assert.Equal(System.Security.Claims.ClaimTypes.Role, options.TokenValidationParameters.RoleClaimType);
        Assert.Equal(typeof(LoggingJwtBearerEvents), options.EventsType);
    }

    [Fact]
    public void AddAuthorizationModule_Deve_Registrar_PolicyProvider_E_Handler()
    {
        var services = new ServiceCollection();

        services.AddAuthorizationModule();

        Assert.Contains(services, d => d.ServiceType == typeof(IAuthorizationPolicyProvider) && d.ImplementationType == typeof(PermissionPolicyProvider));
        Assert.Contains(services, d => d.ServiceType == typeof(IAuthorizationHandler) && d.ImplementationType == typeof(PermissionAuthorizationHandler));
    }

    [Fact]
    public void AddAuthenticationServices_Deve_Registrar_Servicos_Principais()
    {
        var services = new ServiceCollection();

        services.AddAuthenticationServices();

        Assert.Contains(services, d => d.ServiceType == typeof(IAuthService) && d.ImplementationType == typeof(AutenticacaoService));
        Assert.Contains(services, d => d.ServiceType == typeof(IUserService) && d.ImplementationType == typeof(UsuarioAtualService));
        Assert.Contains(services, d => d.ServiceType == typeof(IJwtTokenService) && d.ImplementationType == typeof(JwtTokenService));
        Assert.Contains(services, d => d.ServiceType == typeof(IPasswordHasherService) && d.ImplementationType == typeof(SenhaHasherService));
    }

    [Fact]
    public void AddAuthorizationResultHandlerModule_Deve_Registrar_ResultHandler()
    {
        var services = new ServiceCollection();

        AuthenticationNamespace.AuthorizationResultHandlerModule.AddAuthorizationResultHandlerModule(services);

        Assert.Contains(services, d => d.ServiceType == typeof(IAuthorizationMiddlewareResultHandler) && d.ImplementationType == typeof(ApiAuthorizationMiddlewareResultHandler));
    }

    [Fact]
    public async Task PermissionPolicyProvider_Deve_Retornar_Policies_Padrao_E_Fallback()
    {
        var provider = new PermissionPolicyProvider(Options.Create(new AuthorizationOptions()));

        var defaultPolicy = await provider.GetDefaultPolicyAsync();
        var fallbackPolicy = await provider.GetFallbackPolicyAsync();

        Assert.NotNull(defaultPolicy);
        Assert.Null(fallbackPolicy);
    }

    private static System.Security.Claims.ClaimsPrincipal CriarUsuarioPrincipal(Guid usuarioId)
        => CriarUsuarioPrincipal(usuarioId.ToString());

    private static System.Security.Claims.ClaimsPrincipal CriarUsuarioPrincipal(string usuarioId)
    {
        var identity = new System.Security.Claims.ClaimsIdentity([new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, usuarioId)], "Test");
        return new System.Security.Claims.ClaimsPrincipal(identity);
    }

    private static IConfiguration ConstruirConfiguracao(Dictionary<string, string?>? valores = null)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(valores ?? [])
            .Build();
    }

    private sealed class FakeProfileAuthService : IProfileAuthService
    {
        private readonly bool _possuiPermissao;
        public int PermissaoCalls { get; private set; }

        public FakeProfileAuthService(bool possuiPermissao) => _possuiPermissao = possuiPermissao;

        public Task<IReadOnlyCollection<string>> ObterPerfisAsync(Guid usuarioId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<string>>([]);

        public Task<bool> PossuiPerfilAsync(Guid usuarioId, string perfil, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<IReadOnlyCollection<string>> ObterPermissoesAsync(Guid usuarioId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<string>>([]);

        public Task<bool> PossuiPermissaoAsync(Guid usuarioId, string permissao, CancellationToken cancellationToken = default)
        {
            PermissaoCalls++;
            return Task.FromResult(_possuiPermissao);
        }
    }
}