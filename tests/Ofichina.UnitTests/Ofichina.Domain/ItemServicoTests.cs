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
        var pecaServicoId = Guid.NewGuid();

        var itemServico = ordemServico.AdicionarServico(pecaServicoId);

        Assert.NotEqual(Guid.Empty, itemServico.Id);
        Assert.Equal(pecaServicoId, itemServico.PecaServicoId);
        Assert.Equal(pecaServicoId, itemServico.ServicoId);
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
        var itemServico = ordemServico.AdicionarServico(Guid.NewGuid());
        var pecaServico = CriarPecaServico(Guid.NewGuid(), Guid.NewGuid(), 1, "Filtro de óleo", 25m);
        DefinirPecaServico(itemServico, pecaServico);

        itemServico.AdicionarPeca(pecaServico.PecaId, 2);

        Assert.NotNull(itemServico.UpdatedAt);
        Assert.Single(itemServico.Pecas);
        Assert.Equal(50m, itemServico.ValorTotal);
    }

    [Fact]
    public void Deve_Atualizar_Vinculo_Da_PecaServico()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico(Guid.NewGuid());

        var novoPecaServicoId = Guid.NewGuid();

        itemServico.AtualizarServico(novoPecaServicoId);

        Assert.Equal(novoPecaServicoId, itemServico.PecaServicoId);
        Assert.NotNull(itemServico.UpdatedAt);
    }

    [Fact]
    public void Deve_Impedir_Alteracao_De_Item_Excluido()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico(Guid.NewGuid());

        itemServico.Excluir();

        var exception = Assert.Throws<DomainException>(() =>
            itemServico.AtualizarDados(Guid.NewGuid()));

        Assert.Equal("Não é possível alterar um item de serviço removido.", exception.Message);
    }

    [Fact]
    public void Deve_Remover_Item_Ao_Remover_Peca()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico(Guid.NewGuid());
        var pecaServico = CriarPecaServico(Guid.NewGuid(), Guid.NewGuid(), 1, "Velas", 40m);
        DefinirPecaServico(itemServico, pecaServico);

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

    private static PecaServico CriarPecaServico(Guid servicoId, Guid pecaId, int quantidade, string nomePeca, decimal valorPeca)
    {
        var pecaServico = (PecaServico)Activator.CreateInstance(
            typeof(PecaServico),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: [servicoId, pecaId, quantidade],
            culture: null)!;

        DefinirPeca(pecaServico, new Peca(nomePeca, null, Guid.NewGuid().ToString("N")[..8], valorPeca, 10));
        return pecaServico;
    }

    private static void DefinirPecaServico(ItemServico itemServico, PecaServico pecaServico)
    {
        typeof(ItemServico)
            .GetProperty(nameof(ItemServico.PecaServico), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(itemServico, pecaServico);
    }

    private static void DefinirPeca(PecaServico pecaServico, Peca peca)
    {
        typeof(PecaServico)
            .GetProperty(nameof(PecaServico.Peca), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(pecaServico, peca);
    }
}