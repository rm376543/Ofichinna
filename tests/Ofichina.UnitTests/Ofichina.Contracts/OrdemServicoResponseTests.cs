using Ofichina.Contracts.Extension;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Contracts.Responses.OrdensServico;

namespace Ofichina.UnitTests.Contracts;

public sealed class OrdemServicoResponseTests
{
    [Fact]
    public void OrdemServicoResponse_Deve_Preservar_O_Contrato_Detalhado_Com_ProblemaRelatado()
    {
        var createdAt = new DateTime(2026, 08, 16, 12, 00, 00, DateTimeKind.Utc);
        var updatedAt = new DateTime(2026, 08, 16, 13, 00, 00, DateTimeKind.Utc);

        var response = new OrdemServicoResponse
        {
            OrdemServicoId = Guid.Parse("660e8400-e29b-41d4-a716-446655440000"),
            PessoaId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            VeiculoId = Guid.Parse("770e8400-e29b-41d4-a716-446655440000"),
            ConsultorId = Guid.Parse("880e8400-e29b-41d4-a716-446655440000"),
            MecanicoId = Guid.Parse("990e8400-e29b-41d4-a716-446655440000"),
            Hodometro = 78123,
            ProblemaRelatado = "Barulhos durante a aceleração",
            Status = "CRIADO",
            DataAbertura = createdAt,
            DataFinalizacao = null,
            Observacao = "carro de dev",
            ValorTotal = 1120.00m,
            CreatedAt = createdAt.ToDateString(),
            UpdatedAt = updatedAt.ToDateString(),
            DeletedAt = null,
            Servicos = []
        };

        Assert.Equal("Barulhos durante a aceleração", response.ProblemaRelatado);
        Assert.Equal("CRIADO", response.Status);
        Assert.Equal(78123, response.Hodometro);
        Assert.Equal(createdAt, response.DataAbertura);
        Assert.Equal(updatedAt.ToDateString(), response.UpdatedAt);
        Assert.Empty(response.Servicos);
    }

    [Fact]
    public void OrdemServicoSimplesResponse_Deve_Preservar_O_Contrato_Simplificado_Com_ProblemaRelatado()
    {
        var createdAt = new DateTime(2026, 08, 16, 12, 00, 00, DateTimeKind.Utc);

        var response = new OrdemServicoSimplesResponse
        {
            OrdemServicoId = Guid.Parse("660e8400-e29b-41d4-a716-446655440000"),
            Cliente = "João da Silva",
            Consultor = "Maria Souza",
            ProblemaRelatado = "Barulhos durante a aceleração",
            Status = "CRIADO",
            DataAbetura = "16/08/2026",
            DataFinalizacao = string.Empty,
            Observacao = "carro de dev",
            ValorTotal = "R$ 1.120,00",
            CreatedAt = createdAt.ToDateString()
        };

        Assert.Equal("Barulhos durante a aceleração", response.ProblemaRelatado);
        Assert.Equal("CRIADO", response.Status);
        Assert.Equal("16/08/2026", response.DataAbetura);
        Assert.Equal("R$ 1.120,00", response.ValorTotal);
        Assert.Equal(createdAt.ToDateString(), response.CreatedAt);
    }
}
