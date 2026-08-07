using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Domain;

public sealed class OrcamentoTests
{
    [Fact]
    public void Deve_Criar_Orcamento_Com_Status_Recebida()
    {
        var orcamento = CriarOrcamento();

        Assert.Equal(StatusOrcamento.Recebida, orcamento.Status);
    }

    [Fact]
    public void Deve_Permitir_Adicionar_Servico_Quando_Status_For_Recebida()
    {
        var orcamento = CriarOrcamento(comItens: false);

        var item = orcamento.AdicionarServico(Guid.NewGuid(), Guid.NewGuid(), 1, StatusOrcamento.EmDiagnostico);

        Assert.NotNull(item);
        Assert.Single(orcamento.ItensServico);
        Assert.Equal(StatusOrcamento.Recebida, orcamento.Status);
    }

    [Fact]
    public void Deve_Rejeitar_Alteracao_De_Itens_Quando_Status_Nao_For_Recebida()
    {
        var orcamento = CriarOrcamento();
        orcamento.IniciarDiagnostico();

        var ex = Assert.Throws<DomainException>(() => orcamento.AdicionarServico(Guid.NewGuid(), Guid.NewGuid(), 1, StatusOrcamento.EmDiagnostico));

        Assert.Equal("Não é possível alterar itens nesta etapa do orçamento.", ex.Message);
    }

    [Fact]
    public void Deve_Rejeitar_Inicio_De_Diagnostico_Sem_Itens()
    {
        var orcamento = CriarOrcamento(comItens: false);

        var ex = Assert.Throws<DomainException>(() => orcamento.IniciarDiagnostico());

        Assert.Equal("O orçamento precisa ter ao menos um serviço cadastrado para iniciar o diagnóstico.", ex.Message);
    }

    [Fact]
    public void Deve_Enviar_Orcamento_Apos_Finalizacao_Do_Diagnostico()
    {
        var orcamento = CriarOrcamento();
        orcamento.IniciarDiagnostico();
        orcamento.FinalizarDiagnostico();

        orcamento.EnviarParaCliente();

        Assert.Equal(StatusOrcamento.AguardandoAprovacao, orcamento.Status);
    }

    [Fact]
    public void Deve_Rejeitar_Envio_Quando_Orcamento_Nao_Estiver_Finalizado()
    {
        var orcamento = CriarOrcamento();
        orcamento.IniciarDiagnostico();

        var ex = Assert.Throws<DomainException>(() => orcamento.EnviarParaCliente());

        Assert.Equal("O orçamento precisa estar no status AguardandoAprovacao.", ex.Message);
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
            orcamento.AdicionarServico(Guid.NewGuid(), Guid.NewGuid(), 1, StatusOrcamento.EmDiagnostico);

        return orcamento;
    }
}
