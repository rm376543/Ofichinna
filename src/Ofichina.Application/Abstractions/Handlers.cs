using Ofichina.Contracts.Common;

namespace Ofichina.Application.Abstractions;

/// <summary>
/// Interface para Query Handlers (manipuladores de queries).
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface para Command Handlers (manipuladores de commands).
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Versão simplificada do Command Handler que retorna Result.
/// </summary>
public interface ICommandHandler<in TCommand> where TCommand : ICommand<Result>
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
