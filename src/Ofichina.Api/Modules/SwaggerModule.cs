using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using System.Reflection;
using Ofichina.Contracts.Responses;

namespace Ofichina.Api.Modules;

public static class SwaggerModule
{
    public static IServiceCollection AddSwaggerModule(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Ofichinna API",
                Description = "Api desenvolvida para o Tech Challenge da Pós Tech FIAP",

                TermsOfService = new Uri("https://example.com/terms"),

                Contact = new OpenApiContact
                {
                    Name = "Contato",
                    Url = new Uri("https://example.com/contact")
                },

                License = new OpenApiLicense
                {
                    Name = "Licença de Uso",
                    Url = new Uri("https://example.com/license")
                }
            });

            var assemblies = new[]
            {
                Assembly.GetExecutingAssembly(),
                typeof(ApiResponse).Assembly
            };

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Bearer token. Exemplo: Bearer {seu_token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            foreach (var assembly in assemblies)
            {
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            options.AddSecurityRequirement(document => new() { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] });
        });

        return services;
    }

    public static WebApplication UseSwaggerModule(this WebApplication app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ofichinna API V1");

            // Swagger na raiz da aplicação
            options.RoutePrefix = string.Empty;

            // Outras opções úteis:
            options.DocumentTitle = "Ofichinna API";
            options.DisplayRequestDuration();
        });

        return app;
    }
}