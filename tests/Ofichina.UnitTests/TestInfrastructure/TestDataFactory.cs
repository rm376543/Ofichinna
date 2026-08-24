using Ofichina.Domain.Entities;
using Ofichina.UnitTests.TestInfrastructure.Builders;
using Ofichina.UnitTests.TestInfrastructure.Fakers;

namespace Ofichina.UnitTests.TestInfrastructure;

public static class TestDataFactory
{
    public static class Pessoas
    {
        private static readonly PessoaFaker _faker = new();

        public static Pessoa Criar(Action<Pessoa>? customizar = null)
            => _faker.Criar(customizar);
    }

    public static class Veiculos
    {
        private static readonly VeiculoFaker _faker = new();

        public static Veiculo Criar(Guid? pessoaId = null, Action<Veiculo>? customizar = null)
            => _faker.Criar(pessoaId, customizar);

        public static VeiculoBuilder Builder()
            => new VeiculoBuilder();
    }

    public static class Servicos
    {
        private static readonly ServicoFaker _faker = new();

        public static Servico Criar(Action<Servico>? customizar = null)
            => _faker.Criar(customizar);
    }

    public static class Pecas
    {
        private static readonly PecaFaker _faker = new();

        public static Peca Criar(Action<Peca>? customizar = null)
            => _faker.Criar(customizar);

        public static PecaBuilder Builder()
            => new PecaBuilder();
    }

    public static class ItensServico
    {
        public static ItemServico ParaOrcamento(Guid orcamentoId)
            => ItemServicoFaker.ParaOrcamento(orcamentoId);

        public static ItemServico ParaOrdemServico(Guid ordemServicoId)
            => ItemServicoFaker.ParaOrdemServico(ordemServicoId);
    }

    public static class Orcamentos
    {
        public static OrcamentoBuilder Builder()
            => new OrcamentoBuilder();
    }

    public static class OrdensServico
    {
        public static OrdemServicoBuilder Builder()
            => new OrdemServicoBuilder();
    }
}
