using MediatR;

namespace Ofichina.UnitTests.Api.TestDoubles;

public sealed class FakeMediator : IMediator
{
    private readonly Dictionary<Type, Func<object, object?>> _handlers = new();

    public List<object> Enviados { get; } = [];

    public object? UltimoRequest => Enviados.Count > 0 ? Enviados[^1] : null;

    public void RegistrarResposta<TRequest, TResponse>(TResponse response)
    {
        _handlers[typeof(TRequest)] = _ => response;
    }

    public void RegistrarResposta<TRequest, TResponse>(Func<TRequest, TResponse> handler)
    {
        _handlers[typeof(TRequest)] = request => handler((TRequest)request);
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        Enviados.Add(request);
        return Task.FromResult((TResponse)ObterResposta(request)!);
    }

    public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        return Send((IRequest<TResponse>)request, cancellationToken);
    }

    Task ISender.Send<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        Enviados.Add(request!);
        return Task.CompletedTask;
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        Enviados.Add(request);
        return Task.FromResult(ObterResposta(request));
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task Publish(object notification, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
        => Task.CompletedTask;

    private object? ObterResposta(object request)
    {
        if (_handlers.TryGetValue(request.GetType(), out var handler))
        {
            return handler(request);
        }

        throw new NotSupportedException($"[FakeMediator] Requisição não suportada: {request.GetType().FullName}");
    }
}