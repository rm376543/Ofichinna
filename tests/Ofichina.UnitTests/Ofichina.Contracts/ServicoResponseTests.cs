using Ofichina.Contracts.Responses.Servicos;

namespace Ofichina.UnitTests.Contracts;

public sealed class ServicoResponseTests
{
    [Fact]
    public void ServicoResponse_DeveManterOsValoresInformados()
    {
        var createdAt = new DateTime(2026, 07, 16, 12, 00, 00, DateTimeKind.Utc);

        var response = new ServicoResponse
        {
            ServicoId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Nome = "Troca de óleo",
            Descricao = "Serviço completo",
            Valor = 149.90m,
            Ativo = true,
            CreatedAt = createdAt
        };

        Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), response.ServicoId);
        Assert.Equal("Troca de óleo", response.Nome);
        Assert.Equal("Serviço completo", response.Descricao);
        Assert.Equal(149.90m, response.Valor);
        Assert.True(response.Ativo);
        Assert.Equal(createdAt, response.CreatedAt);
    }
}