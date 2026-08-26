using Microsoft.Extensions.Logging;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Entidades.Handlers;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Tests.UseCases.Agendamentos.Handlers;

public sealed class GetAllAgendamentosPaginadosQueryHandlerTests
{
    private readonly Mock<IAgendamentoRepository> _repositoryMock;
    private readonly Mock<ILogger<GetAllAgendamentosPaginadosQueryHandler>> _loggerMock;
    private readonly GetAllAgendamentosPaginadosQueryHandler _sut;

    public GetAllAgendamentosPaginadosQueryHandlerTests()
    {
        _repositoryMock = new Mock<IAgendamentoRepository>();
        _loggerMock = new Mock<ILogger<GetAllAgendamentosPaginadosQueryHandler>>();

        _sut = new GetAllAgendamentosPaginadosQueryHandler(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalha_QuandoNaoExistemAgendamentos()
    {
        // Arrange
        var pagination = new Pagination();
        var query = new GetAllAgendamentosPaginadosQuery(pagination);
        var cancellationToken = new CancellationTokenSource().Token;

        var pagedResponse = new PagedResponse<VwAgendamentoPessoa>
        {
            Items = [],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 0,
            TotalPages = 0,
            HasNextPage = false,
            HasPreviousPage = false
        };

        _repositoryMock
            .Setup(x => x.GetAllAgendamentosPaginadosAsync(
                pagination,
                cancellationToken))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _sut.HandleAsync(query, cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Nenhum registro encontrado.", result.Error);
        Assert.Empty(result.Errors);

        _repositoryMock.Verify(
            x => x.GetAllAgendamentosPaginadosAsync(
                pagination,
                cancellationToken),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaERegistrarErro_QuandoRepositoryLancaExcecao()
    {
        // Arrange
        var pagination = new Pagination();
        var query = new GetAllAgendamentosPaginadosQuery(pagination);
        var cancellationToken = new CancellationTokenSource().Token;
        var exception = new InvalidOperationException("Erro simulado.");

        _repositoryMock
            .Setup(x => x.GetAllAgendamentosPaginadosAsync(
                pagination,
                cancellationToken))
            .ThrowsAsync(exception);

        // Act
        var result = await _sut.HandleAsync(query, cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível obter os registros.", result.Error);
        Assert.Empty(result.Errors);

        _repositoryMock.Verify(
            x => x.GetAllAgendamentosPaginadosAsync(
                pagination,
                cancellationToken),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString() == "Erro ao listar registros."),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeveEncaminharPaginationECancellationToken_AoRepository()
    {
        // Arrange
        var pagination = new Pagination();
        var query = new GetAllAgendamentosPaginadosQuery(pagination);
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var pagedResponse = new PagedResponse<VwAgendamentoPessoa>
        {
            Items = [],
            PageNumber = 2,
            PageSize = 20,
            TotalCount = 0,
            TotalPages = 0,
            HasNextPage = false,
            HasPreviousPage = true
        };

        _repositoryMock
            .Setup(x => x.GetAllAgendamentosPaginadosAsync(
                pagination,
                cancellationToken))
            .ReturnsAsync(pagedResponse);

        // Act
        await _sut.HandleAsync(query, cancellationToken);

        // Assert
        _repositoryMock.Verify(
            x => x.GetAllAgendamentosPaginadosAsync(
                It.Is<Pagination>(p => ReferenceEquals(p, pagination)),
                It.Is<CancellationToken>(ct => ct == cancellationToken)),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarSucesso_QuandoExistemAgendamentos()
    {
        // Arrange
        var pagination = new Pagination
        {
            PageNumber = 1,
            PageSize = 10
        };

        var item = new VwAgendamentoPessoa
        {
            // Preencha aqui somente se VwAgendamentoPessoa
            // tiver propriedades obrigatórias.
        };

        var pagedResponse = new PagedResponse<VwAgendamentoPessoa>
        {
            Items = [item],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 1,
            TotalPages = 1,
            HasNextPage = false,
            HasPreviousPage = false
        };

        var query = new GetAllAgendamentosPaginadosQuery(pagination);

        _repositoryMock
            .Setup(x => x.GetAllAgendamentosPaginadosAsync(
                pagination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _sut.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Items);

        _repositoryMock.Verify(
            x => x.GetAllAgendamentosPaginadosAsync(
                pagination,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}