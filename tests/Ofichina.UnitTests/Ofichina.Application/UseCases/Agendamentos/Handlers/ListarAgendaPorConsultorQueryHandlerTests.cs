using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Handlers;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.ValueObjects;
using System.Reflection;

namespace Ofichina.UnitTests.Application.UseCases.Agendamentos.Handlers;

public sealed class ListarAgendaPorConsultorQueryHandlerTests
{
    // ============================================================
    // SUCESSO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Agenda_Do_Consultor_Com_Sucesso()
    {
        var consultorId = Guid.NewGuid();
        var data = new DateOnly(2026, 8, 20);

        var query = CriarQuery(
            consultorId,
            data);

        var slotVago = CriarSlot(
            consultorId,
            data,
            new TimeOnly(14, 0));

        var slotAgendado = CriarSlot(
            consultorId,
            data,
            new TimeOnly(9, 30));

        var pessoa = CriarPessoa();

        var veiculo = CriarVeiculo(
            pessoa.Id);

        var agendamento = CriarAgendamento(
            slotAgendado.Id,
            pessoa,
            veiculo);

        var slotRepository = CriarSlotRepository(
            slotVago,
            slotAgendado);

        var agendamentoRepository = CriarAgendamentoRepository(
            agendamento);

        var handler = CriarHandler(
            slotRepository,
            agendamentoRepository);

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var agenda = result.Value.ToList();

        Assert.Equal(2, agenda.Count);

        // --------------------------------------------------------
        // Slot agendado - 09:30
        // --------------------------------------------------------

        Assert.Equal(
            slotAgendado.Id,
            agenda[0].AgendaConsultorId);

        Assert.Equal(
            "09:30",
            agenda[0].Hora);

        Assert.Equal(
            agendamento.Status.ToString(),
            agenda[0].Status);

        Assert.Equal(
            pessoa.Nome,
            agenda[0].ClienteNome);

        Assert.Equal(
            veiculo.Placa.Numero,
            agenda[0].Veiculo);

        // --------------------------------------------------------
        // Slot vago - 14:00
        // --------------------------------------------------------

        Assert.Equal(
            slotVago.Id,
            agenda[1].AgendaConsultorId);

        Assert.Equal(
            "14:00",
            agenda[1].Hora);

        Assert.Equal(
            "VAGO",
            agenda[1].Status);

        Assert.Null(
            agenda[1].ClienteNome);

        Assert.Null(
            agenda[1].Veiculo);

        slotRepository.Verify(
            x => x.GetAllWithIncludesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        agendamentoRepository.Verify(
            x => x.GetAllWithIncludesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ============================================================
    // FILTRO - CONSULTOR
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Ignorar_Slots_De_Outro_Consultor()
    {
        var consultorId = Guid.NewGuid();
        var outroConsultorId = Guid.NewGuid();
        var data = new DateOnly(2026, 8, 20);

        var query = CriarQuery(
            consultorId,
            data);

        var slotDoConsultor = CriarSlot(
            consultorId,
            data,
            new TimeOnly(10, 0));

        var slotOutroConsultor = CriarSlot(
            outroConsultorId,
            data,
            new TimeOnly(11, 0));

        var handler = CriarHandler(
            CriarSlotRepository(
                slotDoConsultor,
                slotOutroConsultor),
            CriarAgendamentoRepository());

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var agenda = result.Value.ToList();

        Assert.Single(agenda);

        Assert.Equal(
            slotDoConsultor.Id,
            agenda[0].AgendaConsultorId);
    }

    // ============================================================
    // FILTRO - DATA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Ignorar_Slots_De_Outra_Data()
    {
        var consultorId = Guid.NewGuid();

        var data = new DateOnly(2026, 8, 20);
        var outraData = new DateOnly(2026, 8, 21);

        var query = CriarQuery(
            consultorId,
            data);

        var slotDaData = CriarSlot(
            consultorId,
            data,
            new TimeOnly(10, 0));

        var slotOutraData = CriarSlot(
            consultorId,
            outraData,
            new TimeOnly(11, 0));

        var handler = CriarHandler(
            CriarSlotRepository(
                slotDaData,
                slotOutraData),
            CriarAgendamentoRepository());

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var agenda = result.Value.ToList();

        Assert.Single(agenda);

        Assert.Equal(
            slotDaData.Id,
            agenda[0].AgendaConsultorId);
    }

    // ============================================================
    // AGENDA VAZIA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Agenda_Vazia_Quando_Nao_Houver_Slots()
    {
        var query = CriarQuery();

        var handler = CriarHandler(
            CriarSlotRepository(),
            CriarAgendamentoRepository());

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        Assert.Empty(
            result.Value);
    }

    // ============================================================
    // STATUS - VAGO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Vago_Quando_Slot_Nao_Possuir_Agendamento()
    {
        var consultorId = Guid.NewGuid();
        var data = new DateOnly(2026, 8, 20);

        var query = CriarQuery(
            consultorId,
            data);

        var slot = CriarSlot(
            consultorId,
            data,
            new TimeOnly(10, 0));

        var handler = CriarHandler(
            CriarSlotRepository(slot),
            CriarAgendamentoRepository());

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var agenda = result.Value.ToList();

        Assert.Single(agenda);

        Assert.Equal(
            "VAGO",
            agenda[0].Status);
    }

    // ============================================================
    // STATUS - AGENDADO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Status_Do_Agendamento()
    {
        var consultorId = Guid.NewGuid();
        var data = new DateOnly(2026, 8, 20);

        var query = CriarQuery(
            consultorId,
            data);

        var slot = CriarSlot(
            consultorId,
            data,
            new TimeOnly(10, 0));

        var pessoa = CriarPessoa();

        var veiculo = CriarVeiculo(
            pessoa.Id);

        var agendamento = CriarAgendamento(
            slot.Id,
            pessoa,
            veiculo);

        var handler = CriarHandler(
            CriarSlotRepository(slot),
            CriarAgendamentoRepository(agendamento));

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var agenda = result.Value.ToList();

        Assert.Single(agenda);

        Assert.Equal(
            StatusAgendamento.AGENDADO.ToString(),
            agenda[0].Status);
    }

    // ============================================================
    // PESSOA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Nome_Da_Pessoa_Do_Agendamento()
    {
        var consultorId = Guid.NewGuid();
        var data = new DateOnly(2026, 8, 20);

        var query = CriarQuery(
            consultorId,
            data);

        var slot = CriarSlot(
            consultorId,
            data,
            new TimeOnly(10, 0));

        var pessoa = CriarPessoa();

        var agendamento = CriarAgendamento(
            slot.Id,
            pessoa,
            null);

        var handler = CriarHandler(
            CriarSlotRepository(slot),
            CriarAgendamentoRepository(agendamento));

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var agenda = result.Value.ToList();

        Assert.Single(agenda);

        Assert.Equal(
            pessoa.Nome,
            agenda[0].ClienteNome);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Null_Quando_Agendamento_Nao_Possuir_Pessoa()
    {
        var consultorId = Guid.NewGuid();
        var data = new DateOnly(2026, 8, 20);

        var query = CriarQuery(
            consultorId,
            data);

        var slot = CriarSlot(
            consultorId,
            data,
            new TimeOnly(10, 0));

        var agendamento = CriarAgendamento(
            slot.Id,
            null,
            null);

        var handler = CriarHandler(
            CriarSlotRepository(slot),
            CriarAgendamentoRepository(agendamento));

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var agenda = result.Value.ToList();

        Assert.Single(agenda);

        Assert.Null(
            agenda[0].ClienteNome);
    }

    // ============================================================
    // VEÍCULO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Placa_Do_Veiculo()
    {
        var consultorId = Guid.NewGuid();
        var data = new DateOnly(2026, 8, 20);

        var query = CriarQuery(
            consultorId,
            data);

        var slot = CriarSlot(
            consultorId,
            data,
            new TimeOnly(10, 0));

        var pessoa = CriarPessoa();

        var veiculo = CriarVeiculo(
            pessoa.Id);

        var agendamento = CriarAgendamento(
            slot.Id,
            pessoa,
            veiculo);

        var handler = CriarHandler(
            CriarSlotRepository(slot),
            CriarAgendamentoRepository(agendamento));

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var agenda = result.Value.ToList();

        Assert.Single(agenda);

        Assert.Equal(
            veiculo.Placa.Numero,
            agenda[0].Veiculo);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Null_Quando_Agendamento_Nao_Possuir_Veiculo()
    {
        var consultorId = Guid.NewGuid();
        var data = new DateOnly(2026, 8, 20);

        var query = CriarQuery(
            consultorId,
            data);

        var slot = CriarSlot(
            consultorId,
            data,
            new TimeOnly(10, 0));

        var pessoa = CriarPessoa();

        var agendamento = CriarAgendamento(
            slot.Id,
            pessoa,
            null);

        var handler = CriarHandler(
            CriarSlotRepository(slot),
            CriarAgendamentoRepository(agendamento));

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var agenda = result.Value.ToList();

        Assert.Single(agenda);

        Assert.Null(
            agenda[0].Veiculo);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Null_Quando_Veiculo_Nao_Possuir_Placa()
    {
        var consultorId = Guid.NewGuid();
        var data = new DateOnly(2026, 8, 20);

        var query = CriarQuery(
            consultorId,
            data);

        var slot = CriarSlot(
            consultorId,
            data,
            new TimeOnly(10, 0));

        var pessoa = CriarPessoa();

        var veiculo = CriarVeiculo(
            pessoa.Id,
            semPlaca: true);

        var agendamento = CriarAgendamento(
            slot.Id,
            pessoa,
            veiculo);

        var handler = CriarHandler(
            CriarSlotRepository(slot),
            CriarAgendamentoRepository(agendamento));

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var agenda = result.Value.ToList();

        Assert.Single(agenda);

        Assert.Null(
            agenda[0].Veiculo);
    }

    // ============================================================
    // EXCEÇÃO - SLOTS
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao_Ao_Buscar_Slots()
    {
        var query = CriarQuery();

        var slotRepository =
            new Mock<IAgendaConsultorRepository>();

        slotRepository
            .Setup(x => x.GetAllWithIncludesAsync(
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro ao buscar slots."));

        var handler = CriarHandler(
            slotRepository,
            CriarAgendamentoRepository());

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Erro ao buscar slots.",
            result.Error);
    }

    // ============================================================
    // EXCEÇÃO - AGENDAMENTOS
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao_Ao_Buscar_Agendamentos()
    {
        var query = CriarQuery();

        var agendamentoRepository =
            new Mock<IAgendamentoRepository>();

        agendamentoRepository
            .Setup(x => x.GetAllWithIncludesAsync(
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro ao buscar agendamentos."));

        var handler = CriarHandler(
            CriarSlotRepository(),
            agendamentoRepository);

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Erro ao buscar agendamentos.",
            result.Error);
    }

    // ============================================================
    // FACTORY - HANDLER
    // ============================================================

    private static ListarAgendaPorConsultorQueryHandler CriarHandler(
        Mock<IAgendaConsultorRepository> slotRepository,
        Mock<IAgendamentoRepository> agendamentoRepository)
    {
        return new ListarAgendaPorConsultorQueryHandler(
            slotRepository.Object,
            agendamentoRepository.Object,
            NullLogger<ListarAgendaPorConsultorQueryHandler>.Instance);
    }

    // ============================================================
    // FACTORY - REPOSITORIES
    // ============================================================

    private static Mock<IAgendaConsultorRepository>
        CriarSlotRepository(
            params AgendaConsultor[] slots)
    {
        var repository =
            new Mock<IAgendaConsultorRepository>();

        repository
            .Setup(x => x.GetAllWithIncludesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(slots);

        return repository;
    }

    private static Mock<IAgendamentoRepository>
        CriarAgendamentoRepository(
            params Agendamento[] agendamentos)
    {
        var repository =
            new Mock<IAgendamentoRepository>();

        repository
            .Setup(x => x.GetAllWithIncludesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(agendamentos);

        return repository;
    }

    // ============================================================
    // FACTORY - QUERY
    // ============================================================

    private static ListarAgendaPorConsultorQuery CriarQuery(
        Guid? consultorId = null,
        DateOnly? data = null)
    {
        return new ListarAgendaPorConsultorQuery
        {
            ConsultorPessoaId = consultorId ?? Guid.NewGuid(),
            Data = data ?? new DateOnly(2026, 8, 20)
        };
    }

    // ============================================================
    // FACTORY - AGENDA CONSULTOR
    // ============================================================

    private static AgendaConsultor CriarSlot(
        Guid consultorId,
        DateOnly data,
        TimeOnly hora)
    {
        var dia = new DiaDisponibilidade(data);

        var horario = CriarHorarioDisponibilidade(hora);

        var slot = new AgendaConsultor(
            dia.Id,
            horario.Id,
            consultorId);

        DefinirPropriedade(
            slot,
            nameof(AgendaConsultor.DiaDisponibilidade),
            dia);

        DefinirPropriedade(
            slot,
            nameof(AgendaConsultor.HorarioDisponibilidade),
            horario);

        return slot;
    }

    // ============================================================
    // FACTORY - HORÁRIO
    // ============================================================

    private static HorarioDisponibilidade CriarHorarioDisponibilidade(
        TimeOnly hora)
    {
        var horario =
            Activator.CreateInstance(
                typeof(HorarioDisponibilidade),
                nonPublic: true)
            as HorarioDisponibilidade;

        if (horario is null)
        {
            throw new InvalidOperationException(
                "Não foi possível criar HorarioDisponibilidade.");
        }

        DefinirPropriedade(
            horario,
            nameof(HorarioDisponibilidade.Hora),
            hora);

        return horario;
    }

    // ============================================================
    // FACTORY - PESSOA
    // ============================================================

    private static Pessoa CriarPessoa()
    {
        var documento = new Cpf(
            "12345678909");

        var telefone = new Telefone(
            "11999999999");

        var endereco = new Endereco(
            "Rua das Flores",
            "100",
            null,
            "Centro",
            "São Paulo",
            "SP",
            new Cep("01001000"));

        return new Pessoa(
            "João da Silva",
            documento,
            telefone,
            endereco,
            Guid.NewGuid());
    }

    // ============================================================
    // FACTORY - VEÍCULO
    // ============================================================

    private static Veiculo CriarVeiculo(
    Guid pessoaId,
    Placa? placa = null,
    bool semPlaca = false)
    {
        var veiculo =
            Activator.CreateInstance(
                typeof(Veiculo),
                nonPublic: true)
            as Veiculo;

        if (veiculo is null)
        {
            throw new InvalidOperationException(
                "Não foi possível criar Veiculo.");
        }

        DefinirPropriedade(
            veiculo,
            nameof(Veiculo.PessoaId),
            pessoaId);

        if (!semPlaca)
        {
            DefinirPropriedade(
                veiculo,
                nameof(Veiculo.Placa),
                placa ?? new Placa("ABC1D23"));
        }
        else
        {
            DefinirPropriedade(
                veiculo,
                nameof(Veiculo.Placa),
                null);
        }

        DefinirPropriedade(
            veiculo,
            nameof(Veiculo.Marca),
            "Toyota");

        DefinirPropriedade(
            veiculo,
            nameof(Veiculo.Modelo),
            "Corolla");

        DefinirPropriedade(
            veiculo,
            nameof(Veiculo.AnoFabricacao),
            2024);

        DefinirPropriedade(
            veiculo,
            nameof(Veiculo.Cor),
            "Preto");

        return veiculo;
    }

    // ============================================================
    // FACTORY - AGENDAMENTO
    // ============================================================

    private static Agendamento CriarAgendamento(
        Guid agendaConsultorId,
        Pessoa? pessoa,
        Veiculo? veiculo)
    {
        var agendamento =
            Activator.CreateInstance(
                typeof(Agendamento),
                nonPublic: true)
            as Agendamento;

        if (agendamento is null)
        {
            throw new InvalidOperationException(
                "Não foi possível criar Agendamento.");
        }

        DefinirPropriedade(
            agendamento,
            nameof(Agendamento.AgendaConsultorId),
            agendaConsultorId);

        DefinirPropriedade(
            agendamento,
            nameof(Agendamento.Cliente),
            pessoa);

        DefinirPropriedade(
            agendamento,
            nameof(Agendamento.Veiculo),
            veiculo);

        DefinirPropriedade(
            agendamento,
            nameof(Agendamento.ClientePessoaId),
            pessoa?.Id ?? Guid.Empty);

        DefinirPropriedade(
            agendamento,
            nameof(Agendamento.VeiculoId),
            veiculo?.Id ?? Guid.Empty);

        DefinirPropriedade(
            agendamento,
            nameof(Agendamento.Status),
            StatusAgendamento.AGENDADO);

        return agendamento;
    }

    // ============================================================
    // REFLECTION HELPER
    // ============================================================

    private static void DefinirPropriedade(
        object objeto,
        string nome,
        object? valor)
    {
        var propriedade = objeto
            .GetType()
            .GetProperty(
                nome,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        if (propriedade is null)
        {
            throw new InvalidOperationException(
                $"A propriedade '{nome}' não foi encontrada " +
                $"em '{objeto.GetType().Name}'.");
        }

        propriedade.SetValue(
            objeto,
            valor);
    }
}