using FluentValidation;
using Ofichina.Contracts.Common;

namespace Ofichina.Api.Middleware;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado durante a execução da requisição.");

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.ContentType = "application/json";

            switch (ex)
            {
                case ValidationException validationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(
                        ApiResponse.FailureResponse(validationException.Errors.Select(x => x.ErrorMessage)));
                    return;

                case KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsJsonAsync(
                        ApiResponse.FailureResponse("Recurso não encontrado."));
                    return;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(
                        ApiResponse.FailureResponse("Você não tem permissão para acessar este recurso."));
                    return;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(
                        ApiResponse.FailureResponse("Não foi possível processar a solicitação."));
                    return;
            }
        }
    }
}
