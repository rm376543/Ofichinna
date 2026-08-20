using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Veiculos.Commands;
using Ofichina.Application.UseCases.Veiculos.Handlers;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ofichina.UnitTests.Application.UseCases.Veiculos.Handlers;

public sealed class CreateVeiculoCommandHandlerTests
{
    // ============================================================
    // PESSOA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Pessoa_Nao_Existir()
    {
        var command = CriarCommand();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pessoa?)null);

        var handler = CriarHandler(
            pessoaRepository: pessoaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Pessoa não encontrada.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Pessoa_Estiver_Excluida()
    {
        var command = CriarCommand();

        var pessoa = CriarPessoa();

        DefinirDeletedAt(pessoa);

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        var handler = CriarHandler(
            pessoaRepository: pessoaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Pessoa não encontrada.",
            result.Error);
    }

    // ============================================================
    // VEÍCULO DUPLICADO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Veiculo_Ja_Existir_Com_Mesma_Placa()
    {
        var command = CriarCommand(
            placa: "ABC1D23");

        var pessoa = CriarPessoa();

        var veiculoExistente = CriarVeiculo(
            pessoa.Id,
            "ABC1D23");

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var veiculoRepository =
            new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetAllAsync(
                includePessoa: true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new[] { veiculoExistente });

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Já existe um veículo cadastrado com esta placa.",
            result.Error);
    }

    // ============================================================
    // VEÍCULO EXISTENTE COM OUTRA PLACA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Criar_Veiculo_Quando_Nao_Houver_Placa_Duplicada()
    {
        var command = CriarCommand(
            placa: "DEF2E34");

        var pessoa = CriarPessoa();

        var outroVeiculo = CriarVeiculo(
            pessoa.Id,
            "ABC1D23");

        var veiculoRepository =
            new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetAllAsync(
                includePessoa: true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new[] { outroVeiculo });

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        veiculoRepository.Verify(
            x => x.AddAsync(
                It.Is<Veiculo>(v =>
                    v.PessoaId == command.PessoaId &&
                    v.Placa.Numero == "DEF2E34" &&
                    v.Marca == command.Marca &&
                    v.Modelo == command.Modelo &&
                    v.AnoFabricacao == command.AnoFabricacao &&
                    v.Cor == command.Cor &&
                    v.Hodometro.Valor == command.Hodometro),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // SUCESSO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Criar_Veiculo_Com_Sucesso()
    {
        var command = CriarCommand();

        var pessoa = CriarPessoa();

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var veiculoRepository =
            new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetAllAsync(
                includePessoa: true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Array.Empty<Veiculo>());

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        veiculoRepository.Verify(
            x => x.AddAsync(
                It.Is<Veiculo>(v =>
                    v.PessoaId == command.PessoaId &&
                    v.Placa.Numero == "ABC1D23"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // COR NULL
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Usar_Cor_Vazia_Quando_Cor_For_Null()
    {
        var command = CriarCommand(
            cor: null);

        var pessoa = CriarPessoa();

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var veiculoRepository =
            new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetAllAsync(
                includePessoa: true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Array.Empty<Veiculo>());

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        veiculoRepository.Verify(
            x => x.AddAsync(
                It.Is<Veiculo>(v =>
                    v.Cor == string.Empty),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // DOMAIN EXCEPTION - PLACA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Placa_For_Invalida()
    {
        var command = CriarCommand(
            placa: "INVALIDA");

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                CriarPessoa());

        var handler = CriarHandler(
            pessoaRepository: pessoaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Placa inválida.",
            result.Error);
    }

    // ============================================================
    // DOMAIN EXCEPTION - HODÔMETRO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Hodometro_For_Invalido()
    {
        var command = CriarCommand(
            hodometro: -1);

        var pessoaRepository =
            CriarPessoaRepository(
                CriarPessoa());

        var veiculoRepository =
            CriarVeiculoRepositoryVazio();

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.NotNull(result.Error);
    }

    // ============================================================
    // DOMAIN EXCEPTION - VEÍCULO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Dados_Do_Veiculo_Foram_Invalidos()
    {
        var command = CriarCommand(
            marca: string.Empty);

        var pessoaRepository =
            CriarPessoaRepository(
                CriarPessoa());

        var veiculoRepository =
            CriarVeiculoRepositoryVazio();

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "A marca deve ser informada.",
            result.Error);
    }

    // ============================================================
    // EXCEPTION
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao_Ao_Buscar_Pessoa()
    {
        var command = CriarCommand();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            pessoaRepository: pessoaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível criar o veículo.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao_Ao_Buscar_Veiculos()
    {
        var command = CriarCommand();

        var pessoa = CriarPessoa();

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var veiculoRepository =
            new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetAllAsync(
                includePessoa: true,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível criar o veículo.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao_Ao_Adicionar_Veiculo()
    {
        var command = CriarCommand();

        var pessoa = CriarPessoa();

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var veiculoRepository =
            CriarVeiculoRepositoryVazio();

        veiculoRepository
            .Setup(x => x.AddAsync(
                It.IsAny<Veiculo>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível criar o veículo.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao_Ao_Salvar()
    {
        var command = CriarCommand();

        var pessoa = CriarPessoa();

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var veiculoRepository =
            CriarVeiculoRepositoryVazio();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        unitOfWork
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível criar o veículo.",
            result.Error);
    }

    // ============================================================
    // FACTORY - HANDLER
    // ============================================================

    private static CreateVeiculoCommandHandler CriarHandler(
        Mock<IVeiculoRepository>? veiculoRepository = null,
        Mock<IRepository<Pessoa>>? pessoaRepository = null,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        return new CreateVeiculoCommandHandler(
            (
                veiculoRepository ??
                new Mock<IVeiculoRepository>()
            ).Object,

            (
                pessoaRepository ??
                new Mock<IRepository<Pessoa>>()
            ).Object,

            (
                unitOfWork ??
                new Mock<IUnitOfWork>()
            ).Object,

            NullLogger<CreateVeiculoCommandHandler>.Instance);
    }

    // ============================================================
    // FACTORY - PESSOA REPOSITORY
    // ============================================================

    private static Mock<IRepository<Pessoa>>
        CriarPessoaRepository(
            Pessoa pessoa)
    {
        var repository =
            new Mock<IRepository<Pessoa>>();

        repository
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        return repository;
    }

    // ============================================================
    // FACTORY - VEÍCULO REPOSITORY
    // ============================================================

    private static Mock<IVeiculoRepository>
        CriarVeiculoRepositoryVazio()
    {
        var repository =
            new Mock<IVeiculoRepository>();

        repository
            .Setup(x => x.GetAllAsync(
                includePessoa: true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Array.Empty<Veiculo>());

        return repository;
    }

    // ============================================================
    // FACTORY - COMMAND
    // ============================================================

    private static CreateVeiculoCommand CriarCommand(
        Guid? pessoaId = null,
        string? placa = "ABC1D23",
        string? marca = "Toyota",
        string? modelo = "Corolla",
        int anoFabricacao = 2024,
        string? cor = "Preto",
        int hodometro = 10000)
    {
        var command =
            (CreateVeiculoCommand)
                RuntimeHelpers.GetUninitializedObject(
                    typeof(CreateVeiculoCommand));

        DefinirPropriedade(
            command,
            nameof(CreateVeiculoCommand.PessoaId),
            pessoaId ?? Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(CreateVeiculoCommand.Placa),
            placa!);

        DefinirPropriedade(
            command,
            nameof(CreateVeiculoCommand.Marca),
            marca!);

        DefinirPropriedade(
            command,
            nameof(CreateVeiculoCommand.Modelo),
            modelo!);

        DefinirPropriedade(
            command,
            nameof(CreateVeiculoCommand.AnoFabricacao),
            anoFabricacao);

        DefinirPropriedade(
            command,
            nameof(CreateVeiculoCommand.Cor),
            cor);

        DefinirPropriedade(
            command,
            nameof(CreateVeiculoCommand.Hodometro),
            hodometro);

        return command;
    }

    // ============================================================
    // FACTORY - PESSOA
    // ============================================================

    private static Pessoa CriarPessoa()
    {
        return new Pessoa(
            "João da Silva",
            new Cpf("12345678909"),
            new Telefone("11999999999"),
            new Endereco(
                "Rua das Flores",
                "100",
                null,
                "Centro",
                "São Paulo",
                "SP",
                new Cep("01001000")),
            Guid.NewGuid());
    }

    // ============================================================
    // FACTORY - VEÍCULO
    // ============================================================

    private static Veiculo CriarVeiculo(
        Guid pessoaId,
        string placa)
    {
        return new Veiculo(
            pessoaId,
            new Placa(placa),
            "Toyota",
            "Corolla",
            2024,
            "Preto",
            new Hodometro(10000));
    }

    // ============================================================
    // REFLECTION - DELETED AT
    // ============================================================

    private static void DefinirDeletedAt(
        object entidade)
    {
        DefinirPropriedade(
            entidade,
            "DeletedAt",
            DateTime.UtcNow);
    }

    // ============================================================
    // REFLECTION - PROPERTY
    // ============================================================

    private static void DefinirPropriedade(
        object objeto,
        string nome,
        object? valor)
    {
        var propriedade =
            objeto.GetType().GetProperty(
                nome,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        if (propriedade is null)
        {
            throw new InvalidOperationException(
                $"A propriedade '{nome}' não foi encontrada " +
                $"em '{objeto.GetType().Name}'.");
        }

        propriedade.SetValue(
            objeto,
            valor);
    }
}