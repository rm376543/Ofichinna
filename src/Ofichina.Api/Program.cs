using Ofichina.Api.Modules;
using Ofichina.Bootstrap;
using DotNetEnv;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerModule();

// Adiciona todos os serviços de infraestrutura e demais
builder.Services.AddBootstrapMiddleware(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerModule();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();