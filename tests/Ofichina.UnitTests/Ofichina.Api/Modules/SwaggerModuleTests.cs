using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Ofichina.Api.Modules;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ofichina.UnitTests.Api.Modules;

public sealed class SwaggerModuleTests
{
    [Fact]
    public void AddSwaggerModule_Deve_Registrar_Servicos_Swagger()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerModule();

        // Assert
        Assert.Contains(
            services,
            x => x.ServiceType ==
                 typeof(Microsoft.AspNetCore.Mvc.ApiExplorer.IApiDescriptionGroupCollectionProvider));

        Assert.Contains(
            services,
            x => x.ServiceType == typeof(IConfigureOptions<SwaggerGeneratorOptions>));
    }

    [Fact]
    public void AddSwaggerModule_Deve_Configurar_SwaggerDoc_E_Seguranca()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddSwaggerModule();

        using var provider = builder.Services.BuildServiceProvider();

        // Act
        var configureOptions =
            provider
                .GetServices<IConfigureOptions<SwaggerGeneratorOptions>>()
                .Single();

        var swaggerOptions = new SwaggerGeneratorOptions();

        configureOptions.Configure(swaggerOptions);

        // Assert
        Assert.True(
            swaggerOptions.SwaggerDocs.ContainsKey("v1"));

        var document = swaggerOptions.SwaggerDocs["v1"];

        Assert.Equal("v1", document.Version);
        Assert.Equal("Ofichinna API", document.Title);
        Assert.Equal(
            "Api desenvolvida para o Tech Challenge da Pós Tech FIAP",
            document.Description);

        Assert.Equal(
            new Uri("https://example.com/terms"),
            document.TermsOfService);

        Assert.NotNull(document.Contact);
        Assert.Equal("Contato", document.Contact.Name);
        Assert.Equal(
            new Uri("https://example.com/contact"),
            document.Contact.Url);

        Assert.NotNull(document.License);
        Assert.Equal("Licença de Uso", document.License.Name);
        Assert.Equal(
            new Uri("https://example.com/license"),
            document.License.Url);

        Assert.True(
            swaggerOptions.SecuritySchemes.ContainsKey(
                JwtBearerDefaults.AuthenticationScheme));

        var securityScheme =
            swaggerOptions.SecuritySchemes[
                JwtBearerDefaults.AuthenticationScheme];

        Assert.Equal("Authorization", securityScheme.Name);

        Assert.Equal(
            "JWT Bearer token. Exemplo: Bearer {seu_token}",
            securityScheme.Description);

        Assert.Equal(
            ParameterLocation.Header,
            securityScheme.In);

        Assert.Equal(
            SecuritySchemeType.Http,
            securityScheme.Type);

        Assert.Equal("bearer", securityScheme.Scheme);
        Assert.Equal("JWT", securityScheme.BearerFormat);

        Assert.NotNull(swaggerOptions.SecurityRequirements);
        Assert.Single(swaggerOptions.SecurityRequirements);
    }

    [Fact]
    public void AddSwaggerModule_Deve_Executar_Configuracao_De_XmlComments()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddSwaggerModule();

        using var provider = builder.Services.BuildServiceProvider();

        var configureOptions =
            provider
                .GetServices<IConfigureOptions<SwaggerGeneratorOptions>>()
                .Single();

        var swaggerOptions = new SwaggerGeneratorOptions();

        // Act
        configureOptions.Configure(swaggerOptions);

        // Assert
        Assert.NotNull(swaggerOptions);
    }

    [Fact]
    public void UseSwaggerModule_Deve_Retornar_A_Mesma_Aplicacao()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddSwaggerModule();

        var app = builder.Build();

        // Act
        var result = app.UseSwaggerModule();

        // Assert
        Assert.Same(app, result);
    }
}