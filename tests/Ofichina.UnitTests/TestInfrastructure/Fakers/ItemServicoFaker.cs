using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public static class ItemServicoFaker
{
    private static readonly Faker Faker = new();

    public static ItemServico ParaOrcamento(Guid orcamentoId)
    {
        var servicoId = Guid.NewGuid();
        Guid? pecaId = null;
        var quantidade = Faker.Random.Int(1, 5);
        return ItemServico.ParaOrcamento(orcamentoId, servicoId, pecaId, quantidade);
    }

    public static ItemServico ParaOrdemServico(Guid ordemServicoId)
    {
        var servicoId = Guid.NewGuid();
        Guid? pecaId = null;
        var quantidade = Faker.Random.Int(1, 5);
        return ItemServico.ParaOrdemServico(ordemServicoId, servicoId, pecaId, quantidade);
    }
}
