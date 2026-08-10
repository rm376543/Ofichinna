using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Domain;

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
        var orcamento = CriarOrcamento();

        orcamento.AtualizarDesconto(10m, false);

        Assert.Equal(10m, orcamento.Desconto);
        Assert.False(orcamento.DescontoEmDinheiro);
        Assert.Equal(0m, orcamento.ValorTotal);
        Assert.Equal(orcamento.ValorTotal, orcamento.ValorTotalDesconto);
    }

    [Fact]
    public void Deve_Calcular_Desconto_Percentual()
    {
        var orcamento = CriarOrcamento();

        orcamento.AtualizarDesconto(10m, true);

        Assert.True(orcamento.DescontoEmDinheiro);
        Assert.Equal(orcamento.ValorBruto * 0.10m, orcamento.ValorBruto - orcamento.ValorTotalDesconto);
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
}
