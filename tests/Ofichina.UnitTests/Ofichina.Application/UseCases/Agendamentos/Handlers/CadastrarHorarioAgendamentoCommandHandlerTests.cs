using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Application.UseCases.Agendamentos.Handlers;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.UseCases.Agendamentos.Handlers;

public sealed class CadastrarHorarioAgendamentoCommandHandlerTests
{
    // ============================================================
    // HandleAsync - Sucesso
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Cadastrar_Horario_Com_Sucesso()
    {
        var repository =
            new Mock<IHorarioDisponibilidadeRepository>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        repository
            .Setup(x => x.BuscarPorHorarioAsync(
                It.IsAny<TimeOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((HorarioDisponibilidade?)null);

        var handler = CriarHandler(
            repository,
            unitOfWork);

        var command = CriarCommand();

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        repository.Verify(
            x => x.BuscarPorHorarioAsync(
                command.Horario,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.AddAsync(
                It.IsAny<HorarioDisponibilidade>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // HandleAsync - Horário existente
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Horario_Ja_Existir()
    {
        var repository =
            new Mock<IHorarioDisponibilidadeRepository>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var horarioExistente =
            new HorarioDisponibilidade(
                TimeOnly.FromTimeSpan(
                    TimeSpan.FromHours(10)));

        repository
            .Setup(x => x.BuscarPorHorarioAsync(
                It.IsAny<TimeOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(horarioExistente);

        var handler = CriarHandler(
            repository,
            unitOfWork);

        var command = CriarCommand();

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "O horário de agendamento já existe.",
            result.Error);

        repository.Verify(
            x => x.BuscarPorHorarioAsync(
                command.Horario,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.AddAsync(
                It.IsAny<HorarioDisponibilidade>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Exception
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrrer_Excecao()
    {
        var repository =
            new Mock<IHorarioDisponibilidadeRepository>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        repository
            .Setup(x => x.BuscarPorHorarioAsync(
                It.IsAny<TimeOnly>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            repository,
            unitOfWork);

        var command = CriarCommand();

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Ocorreu um erro ao cadastrar o horário de agendamento.",
            result.Error);

        repository.Verify(
            x => x.BuscarPorHorarioAsync(
                command.Horario,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.AddAsync(
                It.IsAny<HorarioDisponibilidade>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // Helpers
    // ============================================================

    private static CadastraHorarioAgendamentoCommand CriarCommand()
    => new(
        TimeOnly.FromTimeSpan(
            TimeSpan.FromHours(10)));

    private static CadastrarHorarioAgendamentoCommandHandler CriarHandler(
    Mock<IHorarioDisponibilidadeRepository> repository,
    Mock<IUnitOfWork> unitOfWork)
    => new(
        repository.Object,
        NullLogger<CadastrarHorarioAgendamentoCommandHandler>.Instance,
        unitOfWork.Object);
}