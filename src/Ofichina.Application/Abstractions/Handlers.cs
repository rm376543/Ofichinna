using MediatR;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.Abstractions;

/// <summary>
/// Interface para Query Handlers (manipuladores de queries).
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);

    Task<TResponse> IRequestHandler<TQuery, TResponse>.Handle(TQuery request, CancellationToken cancellationToken)
        => HandleAsync(request, cancellationToken);
}

/// <summary>
/// Interface para Command Handlers (manipuladores de commands).
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);

    Task<TResponse> IRequestHandler<TCommand, TResponse>.Handle(TCommand request, CancellationToken cancellationToken)
        => HandleAsync(request, cancellationToken);
}

/// <summary>
/// Versão simplificada do Command Handler que retorna Result.
/// </summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result> where TCommand : ICommand<Result>
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);

    Task<Result> IRequestHandler<TCommand, Result>.Handle(TCommand request, CancellationToken cancellationToken)
        => HandleAsync(request, cancellationToken);
}
