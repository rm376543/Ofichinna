using Ofichina.Api.Modules;
using Ofichina.Api.Middleware;
using Ofichina.Bootstrap;
using DotNetEnv;
using Serilog;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog como provedor de logging principal
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(
        serverUrl: context.Configuration["Serilog:Seq:ServerUrl"] ?? "http://localhost:5341",
        apiKey: context.Configuration["Serilog:Seq:ApiKey"])
    .WriteTo.File(
        path: context.Configuration["Serilog:File:Path"] ?? "logs/ofichinna-.txt",
        rollingInterval: RollingInterval.Hour,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}"));

builder.Services.AddControllers();

builder.Services.AddSwaggerModule();

// Adiciona todos os serviços de infraestrutura e demais
builder.Services.AddBootstrapMiddleware(builder.Configuration);

var app = builder.Build();// Registrar middleware de Correlation ID (deve ser um dos primeiros)
app.UseCorrelationId();
app.UseMiddleware<ApiExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerModule();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();