using Ofichina.Api.Middleware;

namespace Ofichina.Api.Modules;

/// <summary>
/// Módulo para registro do middleware de Correlation ID.
/// </summary>
public static class CorrelationIdModule
{
    /// <summary>
    /// Registra o middleware de Correlation ID no pipeline da aplicação.
    /// Deve ser chamado antes de outros middlewares que precisem do correlation id nos logs.
    /// </summary>
    /// <param name="app">Instância do WebApplication</param>
    /// <returns>A instância do WebApplication para encadeamento</returns>
    public static WebApplication UseCorrelationId(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }
}
