using Bogus;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class HistoricoStatusFaker
{
    private readonly Faker _faker = new();

    public HistoricoStatus ParaOrcamento(Guid? orcamentoId = null, Guid? alteradoPor = null, string? statusAnterior = null, string? statusNovo = null)
    {
        var oid = orcamentoId ?? _faker.Random.Guid();
        var alterador = alteradoPor ?? _faker.Random.Guid();

        var nomes = Enum.GetNames(typeof(StatusOrcamento));
        var antigo = statusAnterior ?? _faker.PickRandom(nomes);
        var novo = statusNovo ?? _faker.PickRandom(nomes.Where(n => n != antigo).ToArray());

        var data = _faker.Date.Recent();

        return HistoricoStatus.ParaOrcamento(oid, antigo, novo, alterador, data);
    }

    public HistoricoStatus ParaOrdemServico(Guid? ordemServicoId = null, Guid? alteradoPor = null, string? statusAnterior = null, string? statusNovo = null)
    {
        var oid = ordemServicoId ?? _faker.Random.Guid();
        var alterador = alteradoPor ?? _faker.Random.Guid();

        var nomes = Enum.GetNames(typeof(StatusOrdemServico));
        var antigo = statusAnterior ?? _faker.PickRandom(nomes);
        var novo = statusNovo ?? _faker.PickRandom(nomes.Where(n => n != antigo).ToArray());

        var data = _faker.Date.Recent();

        return HistoricoStatus.ParaOrdemServico(oid, antigo, novo, alterador, data);
    }
}
