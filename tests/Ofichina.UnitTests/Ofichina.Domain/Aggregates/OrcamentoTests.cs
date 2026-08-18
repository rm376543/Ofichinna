using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;
using System.Reflection;

namespace Ofichina.UnitTests.Domain.Aggregates;

public sealed class OrcamentoTests
{
    [Fact]
    public void Deve_Criar_Orcamento_Com_Status_Criado()
    {
        var orcamento = CriarOrcamento();

        Assert.Equal(StatusOrcamento.Criado, orcamento.Status);
    }

    [Fact]
    public void Deve_Permitir_Adicionar_Servico_Quando_Status_For_Criado()
    {
        var orcamento = CriarOrcamento(comItens: false);

        var item = orcamento.AdicionarServico(Guid.NewGuid(), Guid.NewGuid(), 1, StatusOrcamento.Criado);

        Assert.NotNull(item);
        Assert.Single(orcamento.ItensServico);
        Assert.Equal(StatusOrcamento.Criado, orcamento.Status);
    }

    [Fact]
    public void Deve_Rejeitar_Alteracao_De_Itens_Quando_Status_Nao_For_Criado()
    {
        var orcamento = CriarOrcamento();
        orcamento.IniciarDiagnostico();

        var ex = Assert.Throws<DomainException>(() => orcamento.AdicionarServico(Guid.NewGuid(), Guid.NewGuid(), 1, StatusOrcamento.Criado));

        Assert.Equal("Não é possível alterar itens nesta etapa do orçamento.", ex.Message);
    }

    [Fact]
    public void Deve_Rejeitar_Inicio_De_Diagnostico_Sem_Itens()
    {
        var orcamento = CriarOrcamento(comItens: false);

        orcamento.IniciarDiagnostico();

        Assert.Equal(StatusOrcamento.EmDiagnostico, orcamento.Status);
    }

    [Fact]
    public void Deve_Enviar_Orcamento_Apos_Finalizacao_Do_Diagnostico()
    {
        var orcamento = CriarOrcamento();
        orcamento.IniciarDiagnostico();
        orcamento.FinalizarDiagnostico();

        Assert.Equal(StatusOrcamento.AguardandoEnvio, orcamento.Status);
        Assert.True(orcamento.ValorBruto.HasValue);
        Assert.True(orcamento.ValorTotal.HasValue);
        Assert.True(orcamento.ValorDesconto.HasValue);
        Assert.True(orcamento.ValorTotalDesconto.HasValue);

        orcamento.EnviarParaCliente();

        Assert.Equal(StatusOrcamento.AguardandoAprovacao, orcamento.Status);
    }

    [Fact]
    public void Deve_Rejeitar_Envio_Quando_Orcamento_Nao_Estiver_Finalizado()
    {
        var orcamento = CriarOrcamento();
        orcamento.IniciarDiagnostico();
        orcamento.FinalizarDiagnostico();

        var ex = Assert.Throws<DomainException>(() => orcamento.Aprovar());

        Assert.Equal("O orçamento precisa estar no status AguardandoAprovacao.", ex.Message);
    }

    [Fact]
    public void Deve_Calcular_Desconto_Fixo_E_Total_Final()
    {
        var orcamento = CriarOrcamentoComValores();

        orcamento.AtualizarDesconto(10m, true);

        Assert.Equal(10m, orcamento.Desconto);
        Assert.True(orcamento.DescontoEmDinheiro);
        Assert.Equal(200m, orcamento.ValorTotal);
        Assert.Equal(10m, orcamento.ValorDesconto);
        Assert.Equal(190m, orcamento.ValorTotalDesconto);
    }

    [Fact]
    public void Deve_Calcular_Desconto_Percentual()
    {
        var orcamento = CriarOrcamentoComValores();

        orcamento.AtualizarDesconto(10m, false);

        Assert.False(orcamento.DescontoEmDinheiro);
        Assert.Equal(20m, orcamento.ValorDesconto);
        Assert.Equal(180m, orcamento.ValorTotalDesconto);
    }

    private static Orcamento CriarOrcamento(bool comItens = true)
    {
        var orcamento = new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(10),
            0,
            "Orçamento teste");

        if (comItens)
            orcamento.AdicionarServico(Guid.NewGuid(), Guid.NewGuid(), 1, StatusOrcamento.Criado);

        return orcamento;
    }

    private static Orcamento CriarOrcamentoComValores()
    {
        var orcamento = CriarOrcamento(comItens: false);

        var item = orcamento.AdicionarServico(Guid.NewGuid(), Guid.NewGuid(), 2, StatusOrcamento.Criado);
        DefinirPropriedade(item, nameof(ItemServico.Servico), new Servico("Serviço teste", null, 100m));
        DefinirPropriedade(item, nameof(ItemServico.Peca), new Peca("Peça teste", null, "PEC-001", 50m, 10));

        return orcamento;
    }

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
    }
}
