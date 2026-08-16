using Ofichina.Contracts.Requests.Autenticacao;
using System.Net;
using System.Net.Http.Json;

namespace Ofichina.IntegrationTests.Api;

public sealed class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_DeveRetornarBadRequest_QuandoEmailForInvalido()
    {
        // Arrange
        var request = new AutenticacaoRequest
        {
            Email = "invalido",
            Senha = "Senha@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_DeveRetornarCorrelationId_QuandoHeaderForInformado()
    {
        // Arrange
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/auth/login")
        {
            Content = JsonContent.Create(new AutenticacaoRequest
            {
                Email = "invalido",
                Senha = "Senha@123"
            })
        };

        request.Headers.Add(
            "X-Correlation-Id",
            "teste-correlation-id-123");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Assert.True(
            response.Headers.TryGetValues(
                "X-Correlation-Id",
                out var correlationIds));

        Assert.Contains(
            "teste-correlation-id-123",
            correlationIds);
    }
}