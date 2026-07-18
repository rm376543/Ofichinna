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
        Assert.Null(servico.DeletedAt);
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
    public void Deve_Rejeitar_Servico_Com_Dados_Invalidos()
    {
        Assert.Throws<DomainException>(() => new Servico(string.Empty, null, 10m));
        Assert.Throws<DomainException>(() => new Servico("Teste", null, 0m));
    }
}