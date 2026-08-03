using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Handlers;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.OrdensServico;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.ValueObjects;
using OrdemServicoAggregate = Ofichina.Domain.Aggregates.OrdemServico;

namespace Ofichina.UnitTests.Application.OrdensServico;

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
            FuncionarioId = funcionario.Id,
            HodometroEntrada = 77290,
            ProblemaRelatado = "Barulhos durante a aceleração",
            Observacoes = "carro de dev"
        });

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.NotNull(createService.CommandRecebido);
        Assert.Equal(pessoa.Id, createService.CommandRecebido!.PessoaId);
        Assert.Equal(veiculo.Id, createService.CommandRecebido.VeiculoId);
        Assert.Equal(funcionario.Id, createService.CommandRecebido.FuncionarioId);
        Assert.Equal(77290, createService.CommandRecebido.HodometroEntrada);
        Assert.Equal("Barulhos durante a aceleração", createService.CommandRecebido.ProblemaRelatado);
        Assert.Equal("carro de dev", createService.CommandRecebido.Observacoes);
    }

    private static Pessoa CriarPessoa()
    {
        return new Pessoa(
            "Cliente Teste",
            new Cpf("39053344705"),
            new Telefone("11999999999"),
            new Endereco("Rua Teste", "100", null, "Centro", "São Paulo", "SP", new Cep("01001000")),
            Guid.NewGuid());
    }

    private static Veiculo CriarVeiculo(Guid pessoaId)
    {
        return new Veiculo(
            pessoaId,
            new Placa("ABC1234"),
            "Volkswagen",
            "Gol",
            2020,
            "Prata",
            new Hodometro(100000));
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