using Microsoft.OpenApi;

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