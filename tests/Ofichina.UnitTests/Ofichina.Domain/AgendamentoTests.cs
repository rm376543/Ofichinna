using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Domain;

public sealed class AgendamentoTests
{
    [Fact]
    public void Deve_Criar_Agendamento_Com_Status_Agendado()
    {
        var clienteId = Guid.NewGuid();
        var agendaId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var agendamento = new Agendamento(clienteId, agendaId, veiculoId, "Revisão completa");

        Assert.NotEqual(Guid.Empty, agendamento.Id);
        Assert.Equal(StatusAgendamento.AGENDADO, agendamento.Status);
        Assert.Equal(clienteId, agendamento.ClientePessoaId);
        Assert.Equal(agendaId, agendamento.AgendaConsultorId);
        Assert.Equal(veiculoId, agendamento.VeiculoId);
        Assert.Equal("Revisão completa", agendamento.Descricao);
    }

    [Fact]
    public void Deve_Iniciar_Agendamento_Quando_Status_For_Agendado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        agendamento.Iniciar();

        Assert.Equal(StatusAgendamento.INICIADO, agendamento.Status);
        Assert.NotNull(agendamento.UpdatedAt);
    }

    [Fact]
    public void Nao_Deve_Iniciar_Agendamento_Quando_Status_Nao_For_Agendado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        agendamento.Iniciar();

        var exception = Assert.Throws<DomainException>(() => agendamento.Iniciar());
        Assert.Contains("'AGENDADO'", exception.Message);
    }

    [Fact]
    public void Nao_Deve_Iniciar_Agendamento_Quando_Status_For_Cancelado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        agendamento.Cancelar();

        var exception = Assert.Throws<DomainException>(() => agendamento.Iniciar());
        Assert.Contains("cancelado", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Deve_Finalizar_Agendamento_Quando_Status_For_Iniciado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        agendamento.Iniciar();

        agendamento.Finalizar();

        Assert.Equal(StatusAgendamento.FINALIZADO, agendamento.Status);
        Assert.NotNull(agendamento.UpdatedAt);
    }

    [Fact]
    public void Nao_Deve_Finalizar_Agendamento_Quando_Status_Nao_For_Iniciado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var exception = Assert.Throws<DomainException>(() => agendamento.Finalizar());
        Assert.Contains("'INICIADO'", exception.Message);
    }

    [Fact]
    public void Deve_Cancelar_Agendamento_Quando_Status_For_Agendado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        agendamento.Cancelar();

        Assert.Equal(StatusAgendamento.CANCELADO, agendamento.Status);
        Assert.NotNull(agendamento.UpdatedAt);
    }

    [Fact]
    public void Deve_Cancelar_Agendamento_Quando_Status_For_Iniciado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        agendamento.Iniciar();

        agendamento.Cancelar();

        Assert.Equal(StatusAgendamento.CANCELADO, agendamento.Status);
    }

    [Fact]
    public void Nao_Deve_Cancelar_Agendamento_Quando_Status_For_Finalizado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        agendamento.Iniciar();
        agendamento.Finalizar();

        var exception = Assert.Throws<DomainException>(() => agendamento.Cancelar());
        Assert.Contains("finalizado", exception.Message);
    }

    [Fact]
    public void Nao_Deve_Cancelar_Agendamento_Quando_Status_For_Cancelado()
    {
        var agendamento = new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        agendamento.Cancelar();

        var exception = Assert.Throws<DomainException>(() => agendamento.Cancelar());
        Assert.Contains("já cancelado", exception.Message);
    }

    [Fact]
    public void Nao_Deve_Criar_Agendamento_Com_Parametros_Invalidos()
    {
        Assert.Throws<DomainException>(() => new Agendamento(Guid.Empty, Guid.NewGuid(), Guid.NewGuid()));
        Assert.Throws<DomainException>(() => new Agendamento(Guid.NewGuid(), Guid.Empty, Guid.NewGuid()));
        Assert.Throws<DomainException>(() => new Agendamento(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty));
    }
}
