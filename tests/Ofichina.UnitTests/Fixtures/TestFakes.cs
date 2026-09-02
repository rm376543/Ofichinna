using Bogus;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using System.Reflection;

namespace Ofichina.UnitTests.Fixtures;

public static class TestFakes
{
    private static readonly Faker Faker = new();

    public static Orcamento CriarOrcamentoAprovado()
    {
        var orcamento = new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(10),
            0,
            Faker.Lorem.Sentence());

        orcamento.AdicionarServico(Guid.NewGuid(), null, 1, StatusOrcamento.Criado);
        orcamento.IniciarDiagnostico();
        orcamento.FinalizarDiagnostico();
        orcamento.EnviarParaCliente();
        orcamento.Aprovar();

        return orcamento;
    }

    public static OrdemServico CriarOrdemServicoEmExecucaoComItem()
    {
        var orcamento = CriarOrcamentoAprovado();
        var ordemServico = OrdemServico.CriarAPartirDoOrcamento(orcamento, null, Guid.NewGuid(), Faker.Random.Int(10000, 99999));
        ordemServico.IniciarExecucao();
        AdicionarItemServico(ordemServico);
        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucaoSemItens()
    {
        var orcamento = CriarOrcamentoAprovado();
        var ordemServico = OrdemServico.CriarAPartirDoOrcamento(orcamento, null, Guid.NewGuid(), Faker.Random.Int(10000, 99999));
        ordemServico.IniciarExecucao();
        return ordemServico;
    }

    public static void AdicionarItemServico(OrdemServico ordemServico)
    {
        var field = typeof(OrdemServico).GetField("_servicos", BindingFlags.Instance | BindingFlags.NonPublic);
        var servicos = (List<ItemServico>)field!.GetValue(ordemServico)!;
        servicos.Add(CriarItemServico(ordemServico.Id));
    }

    public static ItemServico CriarItemServico(Guid ordemServicoId)
    {
        var constructor = typeof(ItemServico).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new Type[] { typeof(Guid?), typeof(Guid?), typeof(Guid), typeof(Guid?), typeof(int) },
            null);

        return (ItemServico)constructor!.Invoke(new object?[] { null, ordemServicoId, Guid.NewGuid(), null, 1 })!;
    }
}
