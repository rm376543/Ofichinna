using Bogus;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

public class OrcamentoBuilder
{
    private readonly Orcamento _orcamento;
    private readonly Faker _faker = new();

    public OrcamentoBuilder()
    {
        var pessoaId = _faker.Random.Guid();
        var veiculoId = _faker.Random.Guid();
        var agendamentoId = _faker.Random.Guid();
        var mecanicoId = _faker.Random.Guid();
        var consultorId = _faker.Random.Guid();
        var dataValidade = DateTime.UtcNow.AddDays(_faker.Random.Int(1, 30));

        _orcamento = new Orcamento(pessoaId, veiculoId, agendamentoId, mecanicoId, consultorId, dataValidade, 0m, null);
    }

    public OrcamentoBuilder ComId(Guid id)
    {
        ReflectionHelpers.DefinirId(_orcamento, id);
        return this;
    }

    public OrcamentoBuilder ComDesconto(decimal desconto)
    {
        _orcamento.AtualizarDesconto(desconto);
        return this;
    }

    public OrcamentoBuilder ComItens(params ItemServico[] itens)
    {
        foreach (var it in itens)
            _orcamento.AdicionarServico(it.ServicoId, it.PecaId, it.Quantidade, StatusOrcamento.Criado);
        return this;
    }

    public OrcamentoBuilder Criado()
    {
        // já está em Criado pelo construtor
        return this;
    }

    public OrcamentoBuilder EmDiagnostico()
    {
        _orcamento.IniciarDiagnostico();
        return this;
    }

    public OrcamentoBuilder FinalizadoDiagnostico()
    {
        _orcamento.IniciarDiagnostico();
        _orcamento.FinalizarDiagnostico();
        return this;
    }

    public OrcamentoBuilder AguardandoEnvio()
    {
        FinalizadoDiagnostico();
        return this;
    }

    public OrcamentoBuilder AguardandoAprovacao()
    {
        FinalizadoDiagnostico();
        _orcamento.EnviarParaCliente();
        return this;
    }

    public OrcamentoBuilder Aprovado()
    {
        AguardandoAprovacao();
        _orcamento.Aprovar();
        return this;
    }

    public OrcamentoBuilder Reprovado()
    {
        AguardandoAprovacao();
        _orcamento.Reprovar();
        return this;
    }

    public Orcamento Build()
    {
        return _orcamento;
    }
}
