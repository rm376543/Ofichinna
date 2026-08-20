using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Handlers;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.UseCases.Agendamentos.Handlers;

public sealed class GetHorariosDisponiveisQueryHandlerTests
{
    // ============================================================
    // HandleAsync - Sucesso
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Horarios_Disponiveis_Com_Sucesso()
    {
        var repository =
            new Mock<IHorarioDisponibilidadeRepository>();

        var pagination = new Pagination(1, 10);

        var horario = new HorarioDisponibilidade(
            TimeOnly.FromTimeSpan(
                TimeSpan.FromHours(10)));

        var horarios = new PagedResponse<HorarioDisponibilidade>
        {
            Items = [horario],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 1,
            TotalPages = 1,
            HasNextPage = false,
            HasPreviousPage = false
        };

        repository
            .Setup(x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(horarios);

        var handler = CriarHandler(repository);

        var query = new GetHorariosDisponiveisQuery(
            pagination);

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Single(result.Value.Items);

        var response = result.Value.Items.First();

        Assert.Equal(
            horario.Id,
            response.HorarioId);

        Assert.Equal(
            horario.Hora,
            response.Horario);

        Assert.True(response.Disponivel);

        repository.Verify(
            x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ============================================================
    // HandleAsync - Nenhum horário
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Nao_Existirem_Horarios()
    {
        var repository =
            new Mock<IHorarioDisponibilidadeRepository>();

        var pagination = new Pagination(1, 10);

        repository
        .Setup(x => x.GetPagedAsync(
            pagination,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(
            (PagedResponse<HorarioDisponibilidade>)null!);

        var handler = CriarHandler(repository);

        var query = new GetHorariosDisponiveisQuery(
            pagination);

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Nenhum horario disponivel encontrado.",
            result.Error);

        Assert.Null(result.Value);

        repository.Verify(
            x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ============================================================
    // HandleAsync - Exceção
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrrer_Excecao()
    {
        var repository =
            new Mock<IHorarioDisponibilidadeRepository>();

        var pagination = new Pagination(1, 10);

        repository
            .Setup(x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(repository);

        var query = new GetHorariosDisponiveisQuery(
            pagination);

        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Ocorreu um erro inesperado ao tentar buscar horarios disponiveis.",
            result.Error);

        Assert.Null(result.Value);

        repository.Verify(
            x => x.GetPagedAsync(
                pagination,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ============================================================
    // Helpers
    // ============================================================

    private static GetHorariosDisponiveisQueryHandler CriarHandler(
        Mock<IHorarioDisponibilidadeRepository> repository)
        => new(
            repository.Object,
            NullLogger<GetHorariosDisponiveisQueryHandler>.Instance);
}