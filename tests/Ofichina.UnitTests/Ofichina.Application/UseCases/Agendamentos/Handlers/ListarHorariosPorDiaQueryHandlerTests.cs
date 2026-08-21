using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Handlers;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.UseCases.Agendamentos.Handlers;

public sealed class ListarHorariosPorDiaQueryHandlerTests
{
    // ============================================================  
    // SUCESSO - ordena, mapeia e marca disponibilidade  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Listar_Horarios_Ordenados_Com_Disponibilidade()
    {
        // Arrange  
        var diaId = Guid.NewGuid();
        var consultorId = Guid.NewGuid();
        var query = new ListarHorariosPorDiaQuery { DiaDisponibilidadeId = diaId };

        // Fora de ordem de propósito para validar o OrderBy(h => h.Hora).  
        var horarioTarde = new HorarioDisponibilidade(new TimeOnly(14, 0));
        var horarioManha = new HorarioDisponibilidade(new TimeOnly(8, 0));

        // Slot para o horário da manhã no dia consultado -> Disponivel = true.  
        // O horário da tarde não tem slot nesse dia -> Disponivel = false.  
        var slotManha = new AgendaConsultor(diaId, horarioManha.Id, consultorId);

        var horarioRepository = new Mock<IHorarioDisponibilidadeRepository>();
        var slotRepository = new Mock<IAgendaConsultorRepository>();

        horarioRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HorarioDisponibilidade> { horarioTarde, horarioManha });

        slotRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AgendaConsultor> { slotManha });

        var handler = CriarHandler(horarioRepository, slotRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(result.Value);

        var itens = result.Value!.ToList();
        Assert.Equal(2, itens.Count);

        // Ordenado por hora: manhã primeiro.  
        Assert.Equal(horarioManha.Id, itens[0].HorarioId);
        Assert.Equal(new TimeOnly(8, 0), itens[0].Horario);
        Assert.True(itens[0].Disponivel);

        Assert.Equal(horarioTarde.Id, itens[1].HorarioId);
        Assert.Equal(new TimeOnly(14, 0), itens[1].Horario);
        Assert.False(itens[1].Disponivel);

        horarioRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        slotRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ============================================================  
    // SUCESSO - nenhum horário retornado  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Lista_Vazia_Quando_Nao_Houver_Horarios()
    {
        // Arrange  
        var query = new ListarHorariosPorDiaQuery { DiaDisponibilidadeId = Guid.NewGuid() };

        var horarioRepository = new Mock<IHorarioDisponibilidadeRepository>();
        var slotRepository = new Mock<IAgendaConsultorRepository>();

        horarioRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HorarioDisponibilidade>());

        slotRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AgendaConsultor>());

        var handler = CriarHandler(horarioRepository, slotRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!);
    }

    // ============================================================  
    // FALHA - exceção inesperada  
    // ============================================================  

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao()
    {
        // Arrange  
        var query = new ListarHorariosPorDiaQuery { DiaDisponibilidadeId = Guid.NewGuid() };

        var horarioRepository = new Mock<IHorarioDisponibilidadeRepository>();
        var slotRepository = new Mock<IAgendaConsultorRepository>();

        horarioRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Falha inesperada."));

        var handler = CriarHandler(horarioRepository, slotRepository);

        // Act  
        var result = await handler.HandleAsync(query);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Falha inesperada.", result.Error);
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static ListarHorariosPorDiaQueryHandler CriarHandler(
        Mock<IHorarioDisponibilidadeRepository> horarioRepository,
        Mock<IAgendaConsultorRepository> slotRepository)
    {
        return new ListarHorariosPorDiaQueryHandler(
            horarioRepository.Object,
            slotRepository.Object,
            NullLogger<ListarHorariosPorDiaQueryHandler>.Instance);
    }
}