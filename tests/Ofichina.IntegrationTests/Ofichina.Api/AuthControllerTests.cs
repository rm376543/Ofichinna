using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Ofichina.Contracts.Requests.Autenticacao;

namespace Ofichina.IntegrationTests.Api;

public sealed class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:9000")
        });
    }

    [Fact]
    public async Task Login_DeveRetornarBadRequest_QuandoEmailForInvalido()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new AutenticacaoRequest
        {
            Email = "invalido",
            Senha = "Senha@123"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_DeveRetornarCorrelationId_QuandoHeaderForInformado()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login")
        {
            Content = JsonContent.Create(new AutenticacaoRequest
            {
                Email = "invalido",
                Senha = "Senha@123"
            })
        };

        request.Headers.Add("X-Correlation-Id", "teste-correlation-id-123");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("X-Correlation-Id", out var correlationIds));
        Assert.Contains("teste-correlation-id-123", correlationIds);
    }
}