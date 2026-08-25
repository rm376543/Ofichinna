using Bogus;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

public class OrdemServicoBuilder
{
    private readonly OrdemServico _ordemServico;
    private readonly Faker _faker = new();

    public OrdemServicoBuilder()
    {
        var pessoaId = _faker.Random.Guid();
        var veiculoId = _faker.Random.Guid();
        var consultorId = _faker.Random.Guid();
        _ordemServico = new OrdemServico(pessoaId, veiculoId, consultorId, 0, _faker.Lorem.Sentence(), null);
    }

    public OrdemServicoBuilder ComId(Guid id)
    {
        ReflectionHelpers.DefinirId(_ordemServico, id);
        return this;
    }

    public OrdemServicoBuilder ComServicos(params ItemServico[] itens)
    {
        foreach (var it in itens)
            _ordemServico.AdicionarServico(it.ServicoId, it.PecaId, it.Quantidade);
        return this;
    }

    public OrdemServicoBuilder Criada()
    {
        return this;
    }

    public OrdemServicoBuilder EmExecucao()
    {
        _ordemServico.IniciarExecucao();
        return this;
    }

    public OrdemServicoBuilder Finalizada()
    {
        _ordemServico.IniciarExecucao();
        _ordemServico.Finalizar();
        return this;
    }

    public OrdemServico Build()
    {
        return _ordemServico;
    }
}
