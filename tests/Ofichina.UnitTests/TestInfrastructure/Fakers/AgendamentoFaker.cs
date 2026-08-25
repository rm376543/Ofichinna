using Bogus;
using Ofichina.Domain.Aggregates;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class AgendamentoFaker
{
    private readonly Faker _faker = new();

    public Agendamento Criar(Guid? clientePessoaId = null, Guid? agendaConsultorId = null, Guid? veiculoId = null, Action<Agendamento>? customizar = null)
    {
        var clienteId = clientePessoaId ?? _faker.Random.Guid();
        var agendaId = agendaConsultorId ?? _faker.Random.Guid();
        var veiculo = veiculoId ?? _faker.Random.Guid();
        var hodometro = FakerHelpers.GerarHodometro();
        var descricao = _faker.Lorem.Sentence();

        var ag = new Agendamento(clienteId, agendaId, veiculo, hodometro, descricao);

        customizar?.Invoke(ag);

        return ag;
    }
}
