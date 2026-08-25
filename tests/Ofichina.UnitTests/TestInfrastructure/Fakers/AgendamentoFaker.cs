using Bogus;
using Ofichina.Domain.Aggregates;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class AgendamentoFaker
{
    private readonly Faker _faker = new();

    public Agendamento Criar(Guid? clientePessoaId = null, Guid? agendaConsultorId = null, Guid? veiculoId = null, Action<Agendamento>? customizar = null)
    {
        var clienteId = clientePessoaId ?? Guid.NewGuid();
        var agendaId = agendaConsultorId ?? Guid.NewGuid();
        var veiculo = veiculoId ?? Guid.NewGuid();
        var hodometro = FakerHelpers.GerarHodometro();
        var descricao = _faker.Lorem.Sentence();

        var ag = new Agendamento(clienteId, agendaId, veiculo, hodometro, descricao);

        customizar?.Invoke(ag);

        return ag;
    }
}
