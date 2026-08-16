using System.Reflection;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Domain;

public sealed class ItemServicoTests
{
    [Fact]
    public void Deve_Criar_Item_Para_Orcamento_E_Calcular_Valor_Total()
    {
        var item = ItemServico.ParaOrcamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2);
        DefinirPropriedade(item, nameof(ItemServico.Servico), new Servico("Mão de obra", null, 100m));
        DefinirPropriedade(item, nameof(ItemServico.Peca), new Peca("Peça", null, "PEC-001", 25m, 10));

        Assert.NotNull(item);
        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.NotNull(item.OrcamentoId);
        Assert.Null(item.OrdemServicoId);
        Assert.Equal(2, item.Quantidade);
        Assert.Equal(150m, item.ValorTotal);
    }

    [Fact]
    public void Deve_Criar_Item_Para_OrdemServico()
    {
        var item = ItemServico.ParaOrdemServico(Guid.NewGuid(), Guid.NewGuid(), null, 1);

        Assert.NotNull(item);
        Assert.Null(item.OrcamentoId);
        Assert.NotNull(item.OrdemServicoId);
        Assert.Equal(1, item.Quantidade);
    }

    [Fact]
    public void Deve_Atualizar_E_Vincular_Item_De_Servico()
    {
        var item = ItemServico.ParaOrcamento(Guid.NewGuid(), Guid.NewGuid(), null, 1);
        var ordemServicoId = Guid.NewGuid();

        item.AtualizarDados(Guid.NewGuid(), Guid.NewGuid(), 3);
        item.VincularAOrdemServico(ordemServicoId);

        Assert.Equal(3, item.Quantidade);
        Assert.Equal(ordemServicoId, item.OrdemServicoId);
        Assert.NotNull(item.UpdatedAt);
    }

    [Fact]
    public void Deve_Rejeitar_Operacoes_Invalidas_E_Item_Removido()
    {
        var item = ItemServico.ParaOrcamento(Guid.NewGuid(), Guid.NewGuid(), null, 1);
        item.Excluir();

        Assert.Throws<DomainException>(() => item.AtualizarDados(Guid.NewGuid(), null, 1));
        Assert.Throws<DomainException>(() => item.VincularAOrdemServico(Guid.NewGuid()));
    }

    [Fact]
    public void Deve_Rejeitar_Criacao_Com_Dados_Invalidos()
    {
        Assert.Throws<DomainException>(() => ItemServico.ParaOrcamento(Guid.NewGuid(), Guid.Empty, null, 1));
        Assert.Throws<DomainException>(() => ItemServico.ParaOrcamento(Guid.NewGuid(), Guid.NewGuid(), null, 0));
        Assert.Throws<DomainException>(() => ItemServico.ParaOrdemServico(Guid.NewGuid(), Guid.Empty, null, 1));
        Assert.Throws<DomainException>(() => ItemServico.ParaOrdemServico(Guid.NewGuid(), Guid.NewGuid(), null, 0));
    }

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
    }
}
