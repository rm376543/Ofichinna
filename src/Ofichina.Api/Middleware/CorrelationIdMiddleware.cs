using Serilog.Context;

namespace Ofichina.Api.Middleware;

/// <summary>
/// Middleware responsável por gerenciar o Correlation ID para rastreamento de requisições.
/// Lê o header 'X-Correlation-Id' ou gera um novo, injeta no contexto de log e retorna na resposta.
/// </summary>
public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        // Tenta ler o Correlation ID do header da requisição
        var correlationId = GetOrGenerateCorrelationId(context);

        // Adiciona o Correlation ID ao contexto de items da requisição
        context.Items[CorrelationIdHeaderName] = correlationId;

        // Adiciona o Correlation ID ao contexto de log do Serilog
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            // Adiciona o Correlation ID ao header de resposta
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(CorrelationIdHeaderName))
                {
                    context.Response.Headers.Append(CorrelationIdHeaderName, correlationId);
                }
                return Task.CompletedTask;
            });

            _logger.LogDebug("Requisição iniciada com CorrelationId: {CorrelationId}", correlationId);

            await _next(context);

            _logger.LogDebug("Requisição finalizada com CorrelationId: {CorrelationId}", correlationId);
        }
    }

    private static string GetOrGenerateCorrelationId(HttpContext context)
    {
        // Tenta ler o Correlation ID do header
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationIdValues))
        {
            var correlationId = correlationIdValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                return correlationId;
            }
        }

        // Se não encontrar, gera um novo GUID
        return Guid.NewGuid().ToString();
    }
}
