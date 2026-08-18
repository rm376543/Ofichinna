using System.Data.Common;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.UnitTests.Infrastructure.Repositories;

public sealed class UnitOfWorkTests
{
    [Fact]
    public async Task SaveChangesAsync_Deve_Persistir_Alteracoes_Do_Contexto()
    {
        await using var context = CriarContexto();
        var unitOfWork = new UnitOfWork(context);

        context.Servicos.Add(new Servico("Troca de óleo", null, 100m));

        var result = await unitOfWork.SaveChangesAsync();

        Assert.Equal(1, result);
        Assert.Equal(1, await context.Servicos.CountAsync());
    }

    [Fact]
    public async Task BeginTransactionAsync_Deve_Lancar_Excecao_Quando_Provider_Nao_Suportar_Transacao()
    {
        await using var context = CriarContexto();
        var unitOfWork = new UnitOfWork(context);

        await Assert.ThrowsAnyAsync<Exception>(() => unitOfWork.BeginTransactionAsync());
    }

    [Fact]
    public async Task CommitTransactionAsync_Deve_Lancar_Excecao_Quando_Nao_Houver_Transacao()
    {
        await using var context = CriarContexto();
        var unitOfWork = new UnitOfWork(context);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.CommitTransactionAsync());

        Assert.Equal("Nenhuma transação ativa foi iniciada.", exception.Message);
    }

    [Fact]
    public async Task RollbackTransactionAsync_Deve_Lancar_Excecao_Quando_Nao_Houver_Transacao()
    {
        await using var context = CriarContexto();
        var unitOfWork = new UnitOfWork(context);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.RollbackTransactionAsync());

        Assert.Equal("Nenhuma transação ativa foi iniciada.", exception.Message);
    }

    [Fact]
    public async Task CommitTransactionAsync_Deve_Comitar_E_Dispor_Transacao()
    {
        await using var context = CriarContexto();
        context.Servicos.Add(new Servico("Troca de óleo", null, 100m));

        var unitOfWork = new UnitOfWork(context);
        var (transaction, proxy) = FakeDbContextTransaction.Create();
        DefinirTransacao(unitOfWork, transaction);

        await unitOfWork.CommitTransactionAsync();

        Assert.Equal(1, proxy.CommitAsyncCalls);
        Assert.Equal(1, proxy.DisposeAsyncCalls + proxy.DisposeCalls);
    }

    [Fact]
    public async Task CommitTransactionAsync_Deve_Retornar_Rollback_Quando_Comit_Falhar()
    {
        await using var context = CriarContexto();
        context.Servicos.Add(new Servico("Troca de óleo", null, 100m));

        var unitOfWork = new UnitOfWork(context);
        var (transaction, proxy) = FakeDbContextTransaction.Create(throwOnCommit: true);
        DefinirTransacao(unitOfWork, transaction);

        await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.CommitTransactionAsync());

        Assert.Equal(1, proxy.CommitAsyncCalls);
        Assert.Equal(1, proxy.RollbackAsyncCalls);
        Assert.Equal(1, proxy.DisposeAsyncCalls + proxy.DisposeCalls);
    }

    [Fact]
    public async Task RollbackTransactionAsync_Deve_Rollback_E_Dispor_Transacao()
    {
        await using var context = CriarContexto();
        var unitOfWork = new UnitOfWork(context);
        var (transaction, proxy) = FakeDbContextTransaction.Create();
        DefinirTransacao(unitOfWork, transaction);

        await unitOfWork.RollbackTransactionAsync();

        Assert.Equal(1, proxy.RollbackAsyncCalls);
        Assert.Equal(1, proxy.DisposeAsyncCalls + proxy.DisposeCalls);
    }

    [Fact]
    public async Task DisposeAsync_Deve_Dispor_Transacao_Quando_Existir()
    {
        await using var context = CriarContexto();
        var unitOfWork = new UnitOfWork(context);
        var (transaction, proxy) = FakeDbContextTransaction.Create();
        DefinirTransacao(unitOfWork, transaction);

        await unitOfWork.DisposeAsync();

        Assert.Equal(1, proxy.DisposeAsyncCalls);
    }

    private static ApplicationDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static void DefinirTransacao(UnitOfWork unitOfWork, IDbContextTransaction transaction)
    {
        var field = typeof(UnitOfWork).GetField("_transaction", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);
        field!.SetValue(unitOfWork, transaction);
    }

    private class FakeDbContextTransaction : DispatchProxy
    {
        public int CommitAsyncCalls { get; private set; }
        public int RollbackAsyncCalls { get; private set; }
        public int DisposeAsyncCalls { get; private set; }
        public int DisposeCalls { get; private set; }
        public bool ThrowOnCommit { get; set; }

        public static (IDbContextTransaction Transaction, FakeDbContextTransaction Proxy) Create(bool throwOnCommit = false)
        {
            var transaction = DispatchProxy.Create<IDbContextTransaction, FakeDbContextTransaction>();
            var proxy = (FakeDbContextTransaction)(object)transaction;
            proxy.ThrowOnCommit = throwOnCommit;
            return (transaction, proxy);
        }

        protected override object? Invoke(MethodInfo targetMethod, object?[]? args)
        {
            switch (targetMethod.Name)
            {
                case nameof(IDbContextTransaction.CommitAsync):
                    CommitAsyncCalls++;
                    if (ThrowOnCommit)
                    {
                        throw new InvalidOperationException("Falha ao comitar transação.");
                    }

                    return Task.CompletedTask;

                case nameof(IDbContextTransaction.RollbackAsync):
                    RollbackAsyncCalls++;
                    return Task.CompletedTask;

                case nameof(IDisposable.Dispose):
                    DisposeCalls++;
                    return null;

                case nameof(IAsyncDisposable.DisposeAsync):
                    DisposeAsyncCalls++;
                    return ValueTask.CompletedTask;

                default:
                    return GetDefaultValue(targetMethod.ReturnType);
            }
        }

        private static object? GetDefaultValue(Type type)
        {
            if (type == typeof(void))
            {
                return null;
            }

            if (type == typeof(Task))
            {
                return Task.CompletedTask;
            }

            if (type == typeof(ValueTask))
            {
                return ValueTask.CompletedTask;
            }

            if (type == typeof(DbTransaction))
            {
                return null;
            }

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}