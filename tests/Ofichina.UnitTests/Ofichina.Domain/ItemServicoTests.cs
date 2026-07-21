using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Domain;

public sealed class ItemServicoTests
{
    [Fact]
    public void Deve_Criar_ItemServico_Com_Valores_Informados()
    {
        var ordemServico = CriarOrdemServico();

        var itemServico = ordemServico.AdicionarServico(
            Guid.NewGuid(),
            "Troca de óleo",
            149.90m);

        Assert.NotEqual(Guid.Empty, itemServico.Id);
        Assert.NotEqual(Guid.Empty, itemServico.ServicoId);
        Assert.Equal(ordemServico.Id, itemServico.OrdemServicoId);
        Assert.Equal("Troca de óleo", itemServico.Descricao);
        Assert.Equal(149.90m, itemServico.Valor);
        Assert.Equal(149.90m, itemServico.ValorTotal);
        Assert.False(itemServico.EstaExcluida());
        Assert.True(itemServico.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Deve_Atualizar_Timestamp_Ao_Adicionar_Peca()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico(
            Guid.NewGuid(),
            "Revisão",
            200m);

        var pecaId = Guid.NewGuid();

        itemServico.AdicionarPeca(
            pecaId,
            "Filtro de óleo",
            2,
            25m);

        Assert.NotNull(itemServico.UpdatedAt);
        Assert.Single(itemServico.Pecas);
        Assert.Equal(50m, itemServico.ValorTotal - itemServico.Valor);
    }

    [Fact]
    public void Deve_Atualizar_Vinculo_Do_Servico()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico(
            Guid.NewGuid(),
            "Diagnóstico inicial",
            90m);

        var novoServicoId = Guid.NewGuid();

        itemServico.AtualizarServico(
            novoServicoId,
            "Diagnóstico atualizado",
            110m);

        Assert.Equal(novoServicoId, itemServico.ServicoId);
        Assert.Equal("Diagnóstico atualizado", itemServico.Descricao);
        Assert.Equal(110m, itemServico.Valor);
        Assert.NotNull(itemServico.UpdatedAt);
    }

    [Fact]
    public void Deve_Impedir_Alteracao_De_Item_Excluido()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico(
            Guid.NewGuid(),
            "Alinhamento",
            120m);

        itemServico.Excluir();

        var exception = Assert.Throws<DomainException>(() =>
            itemServico.AtualizarDados("Balanceamento", 130m));

        Assert.Equal("Não é possível alterar um item de serviço removido.", exception.Message);
    }

    [Fact]
    public void Deve_Impedir_Atualizar_Peca_Removida()
    {
        var ordemServico = CriarOrdemServico();
        var itemServico = ordemServico.AdicionarServico(
            Guid.NewGuid(),
            "Diagnóstico",
            80m);

        itemServico.AdicionarPeca(
            Guid.NewGuid(),
            "Velas",
            1,
            40m);

        var peca = itemServico.Pecas.Single();
        itemServico.RemoverPeca(peca.Id);

        var exception = Assert.Throws<DomainException>(() =>
            itemServico.AtualizarPeca(peca.Id, Guid.NewGuid(), "Velas novas", 2, 45m));

        Assert.Equal("Peça não encontrada.", exception.Message);
    }

    private static OrdemServico CriarOrdemServico()
    {
        return new OrdemServico(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null);
    }
}