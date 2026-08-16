using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Domain;

public sealed class ServicoTests
{
    [Fact]
    public void Deve_Criar_Servico_Com_Valores_Informados()
    {
        var servico = new Servico("Troca de óleo", "Substituição completa do óleo", 149.90m);

        Assert.NotEqual(Guid.Empty, servico.Id);
        Assert.Equal("Troca de óleo", servico.Nome);
        Assert.Equal("Substituição completa do óleo", servico.Descricao);
        Assert.Equal(149.90m, servico.Valor);
        Assert.False(servico.EstaExcluida());
        Assert.True(servico.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Deve_Atualizar_Dados_Do_Servico()
    {
        var servico = new Servico("Balanceamento", null, 89.90m);

        servico.AtualizarDados("Alinhamento", "Ajuste de direção", 129.90m);

        Assert.Equal("Alinhamento", servico.Nome);
        Assert.Equal("Ajuste de direção", servico.Descricao);
        Assert.Equal(129.90m, servico.Valor);
        Assert.NotNull(servico.UpdatedAt);
    }

    [Fact]
    public void Deve_Tratar_Descricao_Como_Nula_Quando_Vazia_Ou_Com_Espacos()
    {
        var servico = new Servico("Balanceamento", "   ", 89.90m);

        Assert.Null(servico.Descricao);

        servico.AtualizarDados("Balanceamento", "   Revisão  ", 99.90m);

        Assert.Equal("Revisão", servico.Descricao);
    }

    [Fact]
    public void Deve_Desativar_E_Reativar_Servico()
    {
        var servico = new Servico("Balanceamento", null, 89.90m);

        servico.Desativar();

        Assert.True(servico.EstaExcluida());

        servico.Desativar();
        servico.Ativar();

        Assert.False(servico.EstaExcluida());
        Assert.NotNull(servico.UpdatedAt);
    }

    [Fact]
    public void Deve_Rejeitar_Servico_Com_Dados_Invalidos()
    {
        Assert.Throws<DomainException>(() => new Servico(string.Empty, null, 10m));
        Assert.Throws<DomainException>(() => new Servico("Teste", null, 0m));
    }
}