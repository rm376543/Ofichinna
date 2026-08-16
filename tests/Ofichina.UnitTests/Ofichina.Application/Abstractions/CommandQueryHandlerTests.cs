using MediatR;
using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.UnitTests.Ofichina.Application.Abstractions;

public sealed class CommandQueryHandlerTests
{
    [Fact]
    public async Task IQueryHandler_Handle_Deve_Delegar_Para_HandleAsync()
    {
        // Arrange
        var query = new TestQuery();
        var handler = new TestQueryHandler();

        var requestHandler =
            (IRequestHandler<TestQuery, Result>)handler;

        // Act
        var result = await requestHandler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Same(query, handler.QueryRecebida);
        Assert.Equal(
            CancellationToken.None,
            handler.CancellationTokenRecebido);
    }

    [Fact]
    public async Task ICommandHandler_Generico_Handle_Deve_Delegar_Para_HandleAsync()
    {
        // Arrange
        var command = new TestCommand();
        var handler = new TestCommandHandler();

        var requestHandler =
            (IRequestHandler<TestCommand, Result>)handler;

        // Act
        var result = await requestHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Same(command, handler.CommandRecebido);
        Assert.Equal(
            CancellationToken.None,
            handler.CancellationTokenRecebido);
    }

    [Fact]
    public async Task ICommandHandler_Simplificado_Handle_Deve_Delegar_Para_HandleAsync()
    {
        // Arrange
        var command = new TestResultCommand();
        var handler = new TestResultCommandHandler();

        var requestHandler =
            (IRequestHandler<TestResultCommand, Result>)handler;

        // Act
        var result = await requestHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Same(command, handler.CommandRecebido);
        Assert.Equal(
            CancellationToken.None,
            handler.CancellationTokenRecebido);
    }

    [Fact]
    public async Task IQueryHandler_Handle_Deve_Propagar_CancellationToken()
    {
        // Arrange
        var query = new TestQuery();
        var handler = new TestQueryHandler();

        using var cts = new CancellationTokenSource();

        var cancellationToken = cts.Token;

        var requestHandler =
            (IRequestHandler<TestQuery, Result>)handler;

        // Act
        await requestHandler.Handle(
            query,
            cancellationToken);

        // Assert
        Assert.Equal(
            cancellationToken,
            handler.CancellationTokenRecebido);
    }

    [Fact]
    public async Task ICommandHandler_Generico_Handle_Deve_Propagar_CancellationToken()
    {
        // Arrange
        var command = new TestCommand();
        var handler = new TestCommandHandler();

        using var cts = new CancellationTokenSource();

        var cancellationToken = cts.Token;

        var requestHandler =
            (IRequestHandler<TestCommand, Result>)handler;

        // Act
        await requestHandler.Handle(
            command,
            cancellationToken);

        // Assert
        Assert.Equal(
            cancellationToken,
            handler.CancellationTokenRecebido);
    }

    [Fact]
    public async Task ICommandHandler_Simplificado_Handle_Deve_Propagar_CancellationToken()
    {
        // Arrange
        var command = new TestResultCommand();
        var handler = new TestResultCommandHandler();

        using var cts = new CancellationTokenSource();

        var cancellationToken = cts.Token;

        var requestHandler =
            (IRequestHandler<TestResultCommand, Result>)handler;

        // Act
        await requestHandler.Handle(
            command,
            cancellationToken);

        // Assert
        Assert.Equal(
            cancellationToken,
            handler.CancellationTokenRecebido);
    }

    private sealed class TestQuery : IQuery<Result>
    {
    }

    private sealed class TestCommand : ICommand<Result>
    {
    }

    private sealed class TestResultCommand : ICommand<Result>
    {
    }

    private sealed class TestQueryHandler
        : IQueryHandler<TestQuery, Result>
    {
        public TestQuery? QueryRecebida { get; private set; }

        public CancellationToken CancellationTokenRecebido { get; private set; }

        public Task<Result> HandleAsync(
            TestQuery query,
            CancellationToken cancellationToken = default)
        {
            QueryRecebida = query;
            CancellationTokenRecebido = cancellationToken;

            return Task.FromResult(Result.Success());
        }
    }

    private sealed class TestCommandHandler
        : ICommandHandler<TestCommand, Result>
    {
        public TestCommand? CommandRecebido { get; private set; }

        public CancellationToken CancellationTokenRecebido { get; private set; }

        public Task<Result> HandleAsync(
            TestCommand command,
            CancellationToken cancellationToken = default)
        {
            CommandRecebido = command;
            CancellationTokenRecebido = cancellationToken;

            return Task.FromResult(Result.Success());
        }
    }

    private sealed class TestResultCommandHandler
        : ICommandHandler<TestResultCommand>
    {
        public TestResultCommand? CommandRecebido { get; private set; }

        public CancellationToken CancellationTokenRecebido { get; private set; }

        public Task<Result> HandleAsync(
            TestResultCommand command,
            CancellationToken cancellationToken = default)
        {
            CommandRecebido = command;
            CancellationTokenRecebido = cancellationToken;

            return Task.FromResult(Result.Success());
        }
    }
}