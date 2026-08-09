using System.Reflection;
using Ofichina.Application.UseCases.Orcamentos.Mappings;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;

namespace Ofichina.UnitTests.Application.Orcamentos;

public sealed class OrcamentoResponseMappingExtensionsTests
{
    [Fact]
    public void ToResponse_Deve_Mapear_Dados_Do_Orcamento_E_Dos_Itens()
    {
        var orcamento = new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(10),
            15m,
            "Avaliar ruído");

        var servico = new Servico("Troca de óleo", null, 120m);
        var peca = new Peca("Filtro de óleo", null, "FILTRO-001", 60m, 10);

        var item = orcamento.AdicionarServico(Guid.NewGuid(), peca.Id, 1, StatusOrcamento.Criado);
        DefinirPropriedade(item, nameof(ItemServico.Servico), servico);
        DefinirPropriedade(item, nameof(ItemServico.Peca), peca);

        orcamento.IniciarDiagnostico();
        orcamento.FinalizarDiagnostico();
        orcamento.AtualizarDesconto(12m);

        var response = orcamento.ToResponse();

        Assert.Equal(orcamento.Id, response.OrcamentoId);
        Assert.Equal(orcamento.PessoaId, response.PessoaId);
        Assert.Equal(orcamento.VeiculoId, response.VeiculoId);
        Assert.Equal(orcamento.AgendamentoId, response.AgendamentoId);
        Assert.Equal(orcamento.MecanicoId, response.MecanicoId);
        Assert.Equal(orcamento.ConsultorId, response.ConsultorId);
        Assert.Equal(DateOnly.FromDateTime(orcamento.DataValidade), response.DataValidade);
        Assert.Equal(12m, response.Desconto);
        Assert.Equal(orcamento.Observacoes, response.Observacoes);
        Assert.Equal("AGUARDANDO_ENVIO", response.Status);
        Assert.Equal(orcamento.DataCriacao, response.DataCriacao);
        Assert.Equal(orcamento.ValorTotal, response.ValorTotal);
        Assert.Single(response.ItensServico);

        var itemResponse = response.ItensServico.Single();
        Assert.Equal(orcamento.Id, itemResponse.OrcamentoId);
        Assert.Single(itemResponse.Servicos);

        var servicoResponse = itemResponse.Servicos.Single();
        Assert.Equal(item.ServicoId, servicoResponse.ServicoId);
        Assert.Equal("Troca de óleo", servicoResponse.Descricao);
        Assert.Equal(120m, servicoResponse.ValorServico);
        Assert.Single(servicoResponse.Pecas);

        var pecaResponse = servicoResponse.Pecas.Single();
        Assert.Equal(peca.Id, pecaResponse.PecaId);
        Assert.Equal("Filtro de óleo", pecaResponse.Descricao);
        Assert.Equal(1, pecaResponse.Quantidade);
        Assert.Equal(60m, pecaResponse.ValorUnitario);
        Assert.Equal(60m, pecaResponse.ValorTotal);
        Assert.Equal(180m, servicoResponse.ValorTotal);
    }

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
    }
}