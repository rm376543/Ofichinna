using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Middleware;

namespace Ofichina.UnitTests.Api.Middleware;

public sealed class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Deve_Propagar_CorrelationId_Do_Header_Da_Requisicao()
    {
        const string correlationId = "teste-correlation-id-123";

        var context = CriarContextoHttp();
        context.Request.Headers["X-Correlation-Id"] = correlationId;

        var middleware = new CorrelationIdMiddleware(async httpContext =>
        {
            await httpContext.Response.StartAsync();
            await httpContext.Response.WriteAsync("ok");
        }, NullLogger<CorrelationIdMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.Equal(correlationId, context.Items["X-Correlation-Id"]);
    }

    [Fact]
    public async Task InvokeAsync_Deve_Gerar_CorrelationId_Quando_Header_Nao_For_Informado()
    {
        var context = CriarContextoHttp();

        var middleware = new CorrelationIdMiddleware(async httpContext =>
        {
            await httpContext.Response.StartAsync();
            await httpContext.Response.WriteAsync("ok");
        }, NullLogger<CorrelationIdMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.True(context.Items.TryGetValue("X-Correlation-Id", out var correlationId));
        Assert.NotNull(correlationId);
    }

    private static DefaultHttpContext CriarContextoHttp()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }
}