using System.Reflection;
using Ofichina.Application.UseCases.OrdensServico.Mappings;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.OrdensServico;

public sealed class OrdemServicoResponseMappingExtensionsTests
{
    [Fact]
    public void ToResponse_Deve_Mapear_ProblemaRelatado_E_Servicos_Da_Ordem()
    {
        var ordemServico = new OrdemServico(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            78123,
            "Barulhos durante a aceleração",
            "carro de dev");

        var servico = new Servico("Troca de óleo", null, 120m);
        var peca = new Peca("Filtro de óleo", null, "FILTRO-001", 60m, 10);

        var item = ordemServico.AdicionarServico(servico.Id, peca.Id, 2);
        DefinirPropriedade(item, nameof(ItemServico.Servico), servico);
        DefinirPropriedade(item, nameof(ItemServico.Peca), peca);

        var response = ordemServico.ToResponse();

        Assert.Equal(ordemServico.Id, response.OrdemServicoId);
        Assert.Equal("Barulhos durante a aceleração", response.ProblemaRelatado);
        Assert.Equal("carro de dev", response.Observacao);
        Assert.Equal("RECEBIDA", response.Status);
        Assert.Single(response.Servicos);

        var itemResponse = Assert.Single(response.Servicos);
        Assert.Equal(ordemServico.Id, itemResponse.OrdemServicoId);
        Assert.Single(itemResponse.Servicos);

        var servicoResponse = Assert.Single(itemResponse.Servicos);
        Assert.Equal(servico.Id, servicoResponse.ServicoId);
        Assert.Equal("Troca de óleo", servicoResponse.Descricao);
        Assert.Equal(120m, servicoResponse.ValorServico);
        Assert.Single(servicoResponse.Pecas);

        var pecaResponse = Assert.Single(servicoResponse.Pecas);
        Assert.Equal(peca.Id, pecaResponse.PecaId);
        Assert.Equal("Filtro de óleo", pecaResponse.Descricao);
        Assert.Equal(2, pecaResponse.Quantidade);
        Assert.Equal(60m, pecaResponse.ValorUnitario);
        Assert.Equal(120m, pecaResponse.ValorTotal);
    }

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
    }
}
