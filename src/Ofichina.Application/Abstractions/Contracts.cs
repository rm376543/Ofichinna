using MediatR;

namespace Ofichina.Application.Abstractions;

/// <summary>
/// Interface base para Queries (leitura de dados).
/// </summary>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Interface base para Commands (escrita de dados).
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
