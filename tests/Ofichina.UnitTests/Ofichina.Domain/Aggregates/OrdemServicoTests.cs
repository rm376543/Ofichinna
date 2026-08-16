using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;
using System.Reflection;

namespace Ofichina.UnitTests.Ofichina.Domain.Aggregates;

public sealed class OrdemServicoTests
{
    [Fact]
    public void CriarAPartirDoOrcamento_Deve_Criar_Ordem_Servico_Com_Status_Criado_E_Hodometro_Da_Aprovacao()
    {
        var orcamento = CriarOrcamentoAprovado();
        var hodometro = 78123;
        var mecanicoReparoId = Guid.NewGuid();

        var ordemServico = OrdemServico.CriarAPartirDoOrcamento(orcamento, null, mecanicoReparoId, hodometro);

        Assert.Equal(StatusOrdemServico.Criado, ordemServico.Status);
        Assert.Equal(hodometro, ordemServico.Hodometro);
        Assert.Equal(orcamento.PessoaId, ordemServico.PessoaId);
        Assert.Equal(orcamento.VeiculoId, ordemServico.VeiculoId);
        Assert.Equal(orcamento.ConsultorId, ordemServico.ConsultorId);
        Assert.Equal(mecanicoReparoId, ordemServico.MecanicoId);
        Assert.Equal(orcamento.Observacoes, ordemServico.ProblemaRelatado);
    }

    [Fact]
    public void IniciarExecucao_Deve_Aceitar_Ordem_Criada_Apos_Aprovacao_Do_Orcamento()
    {
        var orcamento = CriarOrcamentoAprovado();
        var ordemServico = OrdemServico.CriarAPartirDoOrcamento(orcamento, null, Guid.NewGuid(), 78123);

        ordemServico.IniciarExecucao();

        Assert.Equal(StatusOrdemServico.EmExecucao, ordemServico.Status);
    }

    [Fact]
    public void Finalizar_Deve_Permitir_Quando_Houver_Servicos_Ativos()
    {
        var orcamento = CriarOrcamentoAprovado();
        var ordemServico = OrdemServico.CriarAPartirDoOrcamento(orcamento, null, Guid.NewGuid(), 78123);

        AdicionarServicoVinculado(ordemServico);

        ordemServico.IniciarExecucao();
        ordemServico.Finalizar();

        Assert.Equal(StatusOrdemServico.Finalizada, ordemServico.Status);
        Assert.NotNull(ordemServico.DataFinalizacao);
    }

    [Fact]
    public void Finalizar_Deve_Falhar_Quando_Nao_Houver_Servicos_Ativos()
    {
        var orcamento = CriarOrcamentoAprovado();
        var ordemServico = OrdemServico.CriarAPartirDoOrcamento(orcamento, null, Guid.NewGuid(), 78123);

        ordemServico.IniciarExecucao();

        var exception = Assert.Throws<DomainException>(ordemServico.Finalizar);

        Assert.Equal("A ordem de serviço precisa possuir itens cadastrados.", exception.Message);
    }

    private static Orcamento CriarOrcamentoAprovado()
    {
        var orcamento = new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(10),
            0,
            "Barulho na suspensão");

        orcamento.AdicionarServico(Guid.NewGuid(), null, 1, StatusOrcamento.Criado);

        orcamento.IniciarDiagnostico();
        orcamento.FinalizarDiagnostico();
        orcamento.EnviarParaCliente();
        orcamento.Aprovar();

        DefinirAgendamento(orcamento, new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 55220, "Visita técnica"));

        return orcamento;
    }

    private static void DefinirAgendamento(Orcamento orcamento, Agendamento agendamento)
    {
        var property = typeof(Orcamento).GetProperty(nameof(Orcamento.Agendamento), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        property!.SetValue(orcamento, agendamento);
    }

    private static void AdicionarServicoVinculado(OrdemServico ordemServico)
    {
        var field = typeof(OrdemServico).GetField("_servicos", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(field);

        var servicos = (List<ItemServico>)field!.GetValue(ordemServico)!;
        servicos.Add(ConstruirItemServico(ordemServico.Id));
    }

    private static ItemServico ConstruirItemServico(Guid ordemServicoId)
    {
        var constructor = typeof(ItemServico).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            [typeof(Guid?), typeof(Guid?), typeof(Guid), typeof(Guid?), typeof(int)],
            null);

        Assert.NotNull(constructor);

        return (ItemServico)constructor!.Invoke([null, ordemServicoId, Guid.NewGuid(), null, 1]);
    }
}
