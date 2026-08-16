using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Ofichina.Domain.Entities;

public sealed class PecaTests
{
    [Fact]
    public void Deve_Criar_Peca_Com_Valores_Informados_E_Tratar_Espacos()
    {
        var peca = new Peca("  Filtro de óleo  ", "  Filtragem completa  ", "  FIL-001  ", 59.90m, 3);

        Assert.NotEqual(Guid.Empty, peca.Id);
        Assert.Equal("Filtro de óleo", peca.Nome);
        Assert.Equal("Filtragem completa", peca.Descricao);
        Assert.Equal("FIL-001", peca.Codigo);
        Assert.Equal(59.90m, peca.Valor);
        Assert.Equal(3, peca.QuantidadeEstoque);
    }

    [Fact]
    public void Deve_Atualizar_Dados_Da_Peca()
    {
        var peca = new Peca("Filtro", null, "FIL-001", 10m, 1);

        peca.AtualizarDados("  Velas  ", "  Jogo de velas  ", "  VEL-002  ", 25m, 5);

        Assert.Equal("Velas", peca.Nome);
        Assert.Equal("Jogo de velas", peca.Descricao);
        Assert.Equal("VEL-002", peca.Codigo);
        Assert.Equal(25m, peca.Valor);
        Assert.Equal(5, peca.QuantidadeEstoque);
        Assert.NotNull(peca.UpdatedAt);
    }

    [Fact]
    public void Deve_Controlar_Movimentacao_De_Estoque()
    {
        var peca = new Peca("Filtro", null, "FIL-001", 10m, 2);

        peca.EntradaEstoque(3);
        peca.SaidaEstoque(2);
        peca.AjustarQuantidadeEstoque(4);

        Assert.Equal(4, peca.QuantidadeEstoque);
        Assert.NotNull(peca.UpdatedAt);
    }

    [Fact]
    public void Deve_Rejeitar_Movimentacao_De_Estoque_Invalida()
    {
        var peca = new Peca("Filtro", null, "FIL-001", 10m, 2);

        Assert.Throws<DomainException>(() => peca.EntradaEstoque(0));
        Assert.Throws<DomainException>(() => peca.SaidaEstoque(0));
        Assert.Throws<DomainException>(() => peca.SaidaEstoque(3));
        Assert.Throws<DomainException>(() => peca.AjustarQuantidadeEstoque(-1));
    }

    [Fact]
    public void Deve_Rejeitar_Dados_Invalidos()
    {
        Assert.Throws<DomainException>(() => new Peca(string.Empty, null, "FIL-001", 10m, 1));
        Assert.Throws<DomainException>(() => new Peca("Filtro", null, string.Empty, 10m, 1));
        Assert.Throws<DomainException>(() => new Peca("Filtro", null, "FIL-001", 0m, 1));
        Assert.Throws<DomainException>(() => new Peca("Filtro", null, "FIL-001", 10m, -1));
    }
}
