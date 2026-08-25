using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class MotivoRecusaOrcamentoFaker
{
    private readonly Faker _faker = new();

    public MotivoRecusaOrcamento Criar(Guid? orcamentoId = null, Action<MotivoRecusaOrcamento>? customizar = null)
    {
        var oid = orcamentoId ?? Guid.NewGuid();
        var descricao = _faker.Lorem.Sentence();

        var motivo = new MotivoRecusaOrcamento(oid, descricao);

        customizar?.Invoke(motivo);

        return motivo;
    }
}
