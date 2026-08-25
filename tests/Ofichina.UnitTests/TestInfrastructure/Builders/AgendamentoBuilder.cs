using Ofichina.Domain.Aggregates;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

public class AgendamentoBuilder
{
    private Agendamento _agendamento;

    public AgendamentoBuilder()
    {
        _agendamento = TestDataFactory.Agendamentos.Criar();
    }

    public AgendamentoBuilder ComId(Guid id)
    {
        ReflectionHelpers.DefinirId(_agendamento, id);
        return this;
    }

    public AgendamentoBuilder ComClientePessoaId(Guid clienteId)
    {
        ReflectionHelpers.DefinirPropriedade(_agendamento, "ClientePessoaId", clienteId);
        return this;
    }

    public AgendamentoBuilder ComAgendaConsultorId(Guid agendaConsultorId)
    {
        ReflectionHelpers.DefinirPropriedade(_agendamento, "AgendaConsultorId", agendaConsultorId);
        return this;
    }

    public AgendamentoBuilder ComVeiculoId(Guid veiculoId)
    {
        ReflectionHelpers.DefinirPropriedade(_agendamento, "VeiculoId", veiculoId);
        return this;
    }

    public AgendamentoBuilder ComHodometro(int hodometro)
    {
        ReflectionHelpers.DefinirPropriedade(_agendamento, "Hodometro", hodometro);
        return this;
    }

    public AgendamentoBuilder ComDescricao(string descricao)
    {
        ReflectionHelpers.DefinirPropriedade(_agendamento, "Descricao", descricao);
        return this;
    }

    public AgendamentoBuilder Agendado()
    {
        // já está no estado agendado por padrão
        return this;
    }

    public AgendamentoBuilder Iniciado()
    {
        _agendamento.Iniciar();
        return this;
    }

    public AgendamentoBuilder Finalizado()
    {
        _agendamento.Iniciar();
        _agendamento.Finalizar();
        return this;
    }

    public AgendamentoBuilder Cancelado()
    {
        _agendamento.Cancelar();
        return this;
    }

    public Agendamento Build() => _agendamento;
}
