using System.Reflection;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Domain;

public sealed class ItemServicoTests
{
    [Fact]
    public void Deve_Criar_ItemServico_Com_Vinculo_Informado()
    {
        var ordemServico = CriarOrdemServico();

        var itemServico = ordemServico.AdicionarServico();

        Assert.NotEqual(Guid.Empty, itemServico.Id);
        Assert.Equal(Guid.Empty, itemServico.ServicoId);
        Assert.Equal(Guid.Empty, itemServico.ServicoPecaId);
        Assert.Equal(ordemServico.Id, itemServico.OrdemServicoId);
        Assert.Equal(string.Empty, itemServico.Descricao);
        Assert.Equal(0m, itemServico.Valor);
        Assert.Equal(0m, itemServico.ValorTotal);
        Assert.False(itemServico.EstaExcluida());
        Assert.True(itemServico.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Deve_Atualizar_Timestamp_Ao_Adicionar_Peca()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico();
        var pecaServico = CriarServicoPeca(Guid.NewGuid(), Guid.NewGuid(), 1, "Filtro de óleo", 25m);

        itemServico.AdicionarPeca(pecaServico, 2);

        Assert.NotNull(itemServico.UpdatedAt);
        Assert.Single(itemServico.Pecas);
        Assert.Equal(itemServico.Pecas.First().Id, itemServico.ServicoPecaId);
        Assert.Equal(50m, itemServico.ValorTotal);
    }

    [Fact]
    public void Deve_Atualizar_Vinculo_Da_ServicoPeca()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico();

        var servicoPeca = CriarServicoPeca(Guid.NewGuid(), Guid.NewGuid(), 1, "Filtro", 20m);
        itemServico.AdicionarPeca(servicoPeca, 1);

        itemServico.AtualizarServico(servicoPeca.Id);

        Assert.Equal(servicoPeca.Id, itemServico.ServicoPecaId);
        Assert.NotNull(itemServico.UpdatedAt);
    }

    [Fact]
    public void Deve_Impedir_Alteracao_De_Item_Excluido()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico();

        itemServico.Excluir();

        var exception = Assert.Throws<DomainException>(() =>
        itemServico.AtualizarServico(Guid.NewGuid()));

        Assert.Equal("Não é possível alterar um item de serviço removido.", exception.Message);
    }

    [Fact]
    public void Deve_Remover_Item_Ao_Remover_Peca()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico();
        var pecaServico = CriarServicoPeca(Guid.NewGuid(), Guid.NewGuid(), 1, "Velas", 40m);

        itemServico.RemoverPeca(pecaServico.Id);

        Assert.True(itemServico.EstaExcluida());
    }

    private static OrdemServico CriarOrdemServico()
    {
        return new OrdemServico(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            100,
            "Problema relatado de teste",
            null);
    }

    private static ServicoPeca CriarServicoPeca(Guid servicoId, Guid pecaId, int quantidade, string nomePeca, decimal valorPeca)
    {
        var pecaServico = (ServicoPeca)Activator.CreateInstance(
            typeof(ServicoPeca),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: [servicoId, pecaId, quantidade],
            culture: null)!;

        DefinirPeca(pecaServico, new Peca(nomePeca, null, Guid.NewGuid().ToString("N")[..8], valorPeca, 10));
        return pecaServico;
    }

    private static void DefinirPeca(ServicoPeca pecaServico, Peca peca)
    {
        typeof(ServicoPeca)
            .GetProperty(nameof(ServicoPeca.Peca), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(pecaServico, peca);
    }
}