namespace Ofichina.Domain.Common;
/// <summary>
/// Interface para o padrão Unit of Work.
/// Gerencia transações e coordena múltiplos repositórios.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Salva todas as alterações pendentes no banco de dados.
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Inicia uma transação.
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Confirma a transação atual.
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Reverte a transação atual.
    /// </summary>
    Task RollbackTransactionAsync();
}
