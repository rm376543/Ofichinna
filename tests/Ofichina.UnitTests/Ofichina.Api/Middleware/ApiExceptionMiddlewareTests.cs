using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Middleware;

namespace Ofichina.UnitTests.Api.Middleware;

public sealed class ApiExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Deve_Retornar_BadRequest_Quando_Ocorre_ValidationException()
    {
        var middleware = CriarMiddleware(_ => throw new ValidationException(new[]
        {
            new ValidationFailure("Email", "E-mail inválido.")
        }));

        var context = CriarContextoHttp();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_Deve_Retornar_NotFound_Quando_Ocorre_KeyNotFoundException()
    {
        var middleware = CriarMiddleware(_ => throw new KeyNotFoundException());

        var context = CriarContextoHttp();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_Deve_Retornar_Forbid_Quando_Ocorre_UnauthorizedAccessException()
    {
        var middleware = CriarMiddleware(_ => throw new UnauthorizedAccessException());

        var context = CriarContextoHttp();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_Deve_Retornar_InternalServerError_Quando_Ocorre_Exception_Generica()
    {
        var middleware = CriarMiddleware(_ => throw new InvalidOperationException("Falha simulada."));

        var context = CriarContextoHttp();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    private static ApiExceptionMiddleware CriarMiddleware(RequestDelegate next)
        => new(next, NullLogger<ApiExceptionMiddleware>.Instance);

    private static DefaultHttpContext CriarContextoHttp()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }
}