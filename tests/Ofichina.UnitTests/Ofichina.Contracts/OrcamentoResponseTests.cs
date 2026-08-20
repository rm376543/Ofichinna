using Ofichina.Contracts.Responses.Orcamento;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.UnitTests.Ofichina.Contracts;

public sealed class OrcamentoResponseTests
{
    [Fact]
    public void OrcamentoResponse_Deve_Expor_ItensServico_E_Nao_Expor_Propriedade_Legada()
    {
        var createdAt = new DateTime(2026, 07, 16, 12, 00, 00, DateTimeKind.Utc);
        var item = new OrcamentoItemResponse
        {
            OrcamentoId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Servicos =
            [
                new ServicoItemResponse
                {
                    ServicoId = Guid.Parse("11111111-2222-3333-4444-555555555555"),
                    Descricao = "Troca de óleo",
                    ValorServico = 149.90m,
                    ValorTotal = 179.90m
                }
            ]
        };

        var response = new OrcamentoResponse
        {
            OrcamentoId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            MecanicoId = Guid.NewGuid(),
            ConsultorId = Guid.NewGuid(),
            DataValidade = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            Desconto = 15m,
            DescontoEmDinheiro = false,
            ValorDesconto = 15m,
            Status = "CRIADO",
            DataCriacao = createdAt,
            ValorTotal = 179.90m,
            ValorTotalDesconto = 164.90m,
            ItensServico = [item]
        };

        Assert.Single(response.ItensServico);
        Assert.Equal(item, response.ItensServico.First());
        Assert.Equal(179.90m, response.ValorTotal);
        Assert.Equal(164.90m, response.ValorTotalDesconto);
        Assert.Equal(15m, response.ValorDesconto);
        Assert.Null(typeof(OrcamentoResponse).GetProperty("Servicos"));
        Assert.NotNull(typeof(OrcamentoResponse).GetProperty("ItensServico"));
    }
}
