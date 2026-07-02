using Ofichina.Api.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddSwaggerModule();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerModule();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();