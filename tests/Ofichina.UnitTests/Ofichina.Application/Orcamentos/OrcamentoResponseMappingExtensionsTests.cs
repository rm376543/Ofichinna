using System.Reflection;
using Ofichina.Application.UseCases.Orcamentos.Mappings;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;

namespace Ofichina.UnitTests.Application.Orcamentos;

public sealed class OrcamentoResponseMappingExtensionsTests
{
    [Fact]
    public void ToResponse_Deve_Mapear_Dados_Do_Orcamento_Com_Servicos_Agrupados()
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

        var servicoTrocaOleo = new Servico("Troca de óleo", null, 120m);
        var pecaFiltro = new Peca("Filtro de óleo", null, "FILTRO-001", 60m, 10);
        var pecaVedacao = new Peca("Anel de vedação", null, "VED-001", 15m, 10);
        var servicoAlinhamento = new Servico("Alinhamento", null, 80m);

        var itemTrocaOleoFiltro = orcamento.AdicionarServico(servicoTrocaOleo.Id, pecaFiltro.Id, 1, StatusOrcamento.Criado);
        DefinirPropriedade(itemTrocaOleoFiltro, nameof(ItemServico.Servico), servicoTrocaOleo);
        DefinirPropriedade(itemTrocaOleoFiltro, nameof(ItemServico.Peca), pecaFiltro);

        var itemTrocaOleoVedacao = orcamento.AdicionarServico(servicoTrocaOleo.Id, pecaVedacao.Id, 2, StatusOrcamento.Criado);
        DefinirPropriedade(itemTrocaOleoVedacao, nameof(ItemServico.Servico), servicoTrocaOleo);
        DefinirPropriedade(itemTrocaOleoVedacao, nameof(ItemServico.Peca), pecaVedacao);

        var itemAlinhamento = orcamento.AdicionarServico(servicoAlinhamento.Id, null, 1, StatusOrcamento.Criado);
        DefinirPropriedade(itemAlinhamento, nameof(ItemServico.Servico), servicoAlinhamento);

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
        Assert.Equal(orcamento.ValorTotalDesconto, response.ValorTotalDesconto);
        Assert.Single(response.ItensServico);

        var itemResponse = response.ItensServico.Single();
        Assert.Equal(orcamento.Id, itemResponse.OrcamentoId);
        Assert.Equal(2, itemResponse.Servicos.Count);

        var trocaOleo = itemResponse.Servicos.Single(x => x.ServicoId == servicoTrocaOleo.Id);
        Assert.Equal("Troca de óleo", trocaOleo.Descricao);
        Assert.Equal(120m, trocaOleo.ValorServico);
        Assert.Equal(2, trocaOleo.Pecas.Count);
        Assert.Equal(210m, trocaOleo.ValorTotal);

        var filtro = trocaOleo.Pecas.Single(x => x.PecaId == pecaFiltro.Id);
        Assert.Equal("Filtro de óleo", filtro.Descricao);
        Assert.Equal(1, filtro.Quantidade);
        Assert.Equal(60m, filtro.ValorUnitario);
        Assert.Equal(60m, filtro.ValorTotal);

        var vedacao = trocaOleo.Pecas.Single(x => x.PecaId == pecaVedacao.Id);
        Assert.Equal("Anel de vedação", vedacao.Descricao);
        Assert.Equal(2, vedacao.Quantidade);
        Assert.Equal(15m, vedacao.ValorUnitario);
        Assert.Equal(30m, vedacao.ValorTotal);

        var alinhamento = itemResponse.Servicos.Single(x => x.ServicoId == servicoAlinhamento.Id);
        Assert.Empty(alinhamento.Pecas);
        Assert.Equal(80m, alinhamento.ValorTotal);
    }

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
    }
}