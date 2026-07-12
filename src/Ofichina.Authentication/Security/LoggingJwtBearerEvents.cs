using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;

namespace Ofichina.Authentication.Security;

public sealed class LoggingJwtBearerEvents : JwtBearerEvents
{
    private readonly ILogger<LoggingJwtBearerEvents> _logger;

    public LoggingJwtBearerEvents(ILogger<LoggingJwtBearerEvents> logger)
    {
        _logger = logger;
    }

    public override Task MessageReceived(MessageReceivedContext context)
    {
        _logger.LogDebug("Evento JWT: MessageReceived");
        return Task.CompletedTask;
    }

    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        _logger.LogWarning(context.Exception, "Evento JWT: AuthenticationFailed");
        return Task.CompletedTask;
    }

    public override Task TokenValidated(TokenValidatedContext context)
    {
        _logger.LogInformation("Evento JWT: TokenValidated");
        return Task.CompletedTask;
    }

    public override Task Challenge(JwtBearerChallengeContext context)
    {
        _logger.LogWarning("Evento JWT: Challenge");
        return Task.CompletedTask;
    }
}