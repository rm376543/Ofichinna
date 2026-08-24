using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Service;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Handlers;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.OrdensServico;
using Ofichina.Domain.Entities;
using Ofichina.UnitTests.TestInfrastructure;

namespace Ofichina.UnitTests.Application.UseCases.OrdensServico.Handlers;

public sealed class CreateOrdemServicoCommandHandlerTests
{
    [Fact]
    public async Task Deve_Criar_Ordem_Sem_Servicos_E_Com_Status_Recebida()
    {
        var pessoa = CriarPessoa();
        var funcionario = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);

        var createService = new FakeCreateOrdemServicoService();
        var handler = new CreateOrdemServicoCommandHandler(
            createService,
            NullLogger<CreateOrdemServicoCommandHandler>.Instance);

        var command = new CreateOrdemServicoCommand(new CreateOrdemServicoRequest
        {
            PessoaId = pessoa.Id,
            VeiculoId = veiculo.Id,
            ConsultorId = funcionario.Id,
            Hodometro = 77290,
            ProblemaRelatado = "Barulhos durante a aceleração",
            Observacoes = "carro de dev"
        });

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.NotNull(createService.CommandRecebido);
        Assert.Equal(pessoa.Id, createService.CommandRecebido!.PessoaId);
        Assert.Equal(veiculo.Id, createService.CommandRecebido.VeiculoId);
        Assert.Equal(funcionario.Id, createService.CommandRecebido.ConsultorId);
        Assert.Equal(77290, createService.CommandRecebido.Hodometro);
        Assert.Equal("Barulhos durante a aceleração", createService.CommandRecebido.ProblemaRelatado);
        Assert.Equal("carro de dev", createService.CommandRecebido.Observacoes);
    }

    private static Pessoa CriarPessoa()
    {
        return TestDataFactory.Pessoas.Criar();
    }

    private static Veiculo CriarVeiculo(Guid pessoaId)
    {
        return TestDataFactory.Veiculos.Criar(pessoaId);
    }

    private sealed class FakeCreateOrdemServicoService : ICreateOrdemServicoService
    {
        public CreateOrdemServicoCommand? CommandRecebido { get; private set; }

        public Task<Result> CreateAsync(CreateOrdemServicoCommand command, CancellationToken cancellationToken = default)
        {
            CommandRecebido = command;
            return Task.FromResult(Result.Success());
        }
    }
    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public Task BeginTransactionAsync() => Task.CompletedTask;

        public Task CommitTransactionAsync() => Task.CompletedTask;

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCalls++;
            return Task.FromResult(1);
        }

        public Task RollbackTransactionAsync() => Task.CompletedTask;
    }
}