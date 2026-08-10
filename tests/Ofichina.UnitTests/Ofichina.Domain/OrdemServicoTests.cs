using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Enums;
using System.Reflection;

namespace Ofichina.UnitTests.Domain;

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
}
