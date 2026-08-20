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

public sealed class UpdateVeiculoCommandHandlerTests
{
    // ============================================================
    // VEÍCULO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Veiculo_Nao_Existir()
    {
        var command = CriarCommand();

        var veiculoRepository =
            new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Veiculo?)null);

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Veículo não encontrado.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Veiculo_Estiver_Excluido()
    {
        var command = CriarCommand();

        var veiculo = CriarVeiculo(
            command.PessoaId,
            "ABC1D23");

        DefinirDeletedAt(veiculo);

        var veiculoRepository =
            CriarVeiculoRepository(veiculo);

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Veículo não encontrado.",
            result.Error);
    }

    // ============================================================
    // PESSOA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Pessoa_Nao_Existir()
    {
        var command = CriarCommand();

        var veiculo = CriarVeiculo(
            command.PessoaId,
            "ABC1D23");

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pessoa?)null);

        var veiculoRepository =
            CriarVeiculoRepository(veiculo);

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
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

        var pessoa = CriarPessoa(
            command.PessoaId);

        DefinirDeletedAt(pessoa);

        var veiculo = CriarVeiculo(
            command.PessoaId,
            "ABC1D23");

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var veiculoRepository =
            CriarVeiculoRepository(veiculo);

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
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
    // PLACA DUPLICADA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Placa_Pertencer_A_Outro_Veiculo()
    {
        var pessoaId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var command = CriarCommand(
            veiculoId: veiculoId,
            pessoaId: pessoaId,
            placa: "XYZ9K99");

        var pessoa = CriarPessoa(pessoaId);

        var veiculo = CriarVeiculo(
            pessoaId,
            "ABC1D23");

        DefinirId(
            veiculo,
            veiculoId);

        var outroVeiculo = CriarVeiculo(
            pessoaId,
            "XYZ9K99");

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var veiculoRepository =
            new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        veiculoRepository
            .Setup(x => x.GetAllAsync(
                includePessoa: true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new[]
                {
                    veiculo,
                    outroVeiculo
                });

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Já existe outro veículo cadastrado com esta placa.",
            result.Error);
    }

    // ============================================================
    // PLACA DO PRÓPRIO VEÍCULO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Permitir_Atualizacao_Quando_Placa_Pertencer_Ao_Proprio_Veiculo()
    {
        var pessoaId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var command = CriarCommand(
            veiculoId: veiculoId,
            pessoaId: pessoaId,
            placa: "ABC1D23");

        var pessoa = CriarPessoa(pessoaId);

        var veiculo = CriarVeiculo(
            pessoaId,
            "ABC1D23");

        DefinirId(
            veiculo,
            veiculoId);

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var veiculoRepository =
            new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        veiculoRepository
            .Setup(x => x.GetAllAsync(
                includePessoa: true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new[]
                {
                veiculo
                });

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(
            result.IsSuccess,
            $"O handler retornou erro: {result.Error}");

        veiculoRepository.Verify(
            x => x.UpdateAsync(
                It.Is<Veiculo>(v =>
                    v.Id == command.VeiculoId &&
                    v.PessoaId == command.PessoaId &&
                    v.Placa.Numero == command.Placa &&
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
    public async Task HandleAsync_Deve_Atualizar_Veiculo_Com_Sucesso()
    {
        var pessoaId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var command = CriarCommand(
            veiculoId: veiculoId,
            pessoaId: pessoaId,
            placa: "DEF2E34",
            marca: "Honda",
            modelo: "Civic",
            anoFabricacao: 2025,
            cor: "Prata",
            hodometro: 25000);

        var pessoa = CriarPessoa(pessoaId);

        var veiculo = CriarVeiculo(
            pessoaId,
            "ABC1D23");

        DefinirId(
            veiculo,
            veiculoId);

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var veiculoRepository =
            new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        veiculoRepository
            .Setup(x => x.GetAllAsync(
                includePessoa: true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new[]
                {
                veiculo
                });

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: pessoaRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(
            result.IsSuccess,
            $"O handler retornou erro: {result.Error}");

        veiculoRepository.Verify(
            x => x.UpdateAsync(
                It.Is<Veiculo>(v =>
                    v.Id == command.VeiculoId &&
                    v.PessoaId == command.PessoaId &&
                    v.Placa.Numero == command.Placa &&
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
    // DOMAIN EXCEPTION - PLACA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Placa_For_Invalida()
    {
        var command = CriarCommand(
            placa: "INVALIDA");

        var handler = CriarHandler();

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
        var pessoaId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var command = CriarCommand(
            veiculoId: veiculoId,
            pessoaId: pessoaId,
            hodometro: -1);

        var pessoa = CriarPessoa(pessoaId);

        var veiculo = CriarVeiculo(
            pessoaId,
            "ABC1D23");

        DefinirId(
            veiculo,
            veiculoId);

        var pessoaRepository =
            CriarPessoaRepository(pessoa);

        var veiculoRepository =
            CriarVeiculoRepository(veiculo);

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
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Marca_For_Invalida()
    {
        var pessoaId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var command = CriarCommand(
            veiculoId: veiculoId,
            pessoaId: pessoaId,
            marca: string.Empty);

        var pessoa = CriarPessoa(pessoaId);

        var veiculo = CriarVeiculo(
            pessoaId,
            "ABC1D23");

        DefinirId(
            veiculo,
            veiculoId);

        var handler = CriarHandler(
            veiculoRepository: CriarVeiculoRepository(veiculo),
            pessoaRepository: CriarPessoaRepository(pessoa));

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "A marca deve ser informada.",
            result.Error);
    }

    // ============================================================
    // DOMAIN EXCEPTION - MODELO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Modelo_For_Invalido()
    {
        var pessoaId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var command = CriarCommand(
            veiculoId: veiculoId,
            pessoaId: pessoaId,
            modelo: string.Empty);

        var pessoa = CriarPessoa(pessoaId);

        var veiculo = CriarVeiculo(
            pessoaId,
            "ABC1D23");

        DefinirId(
            veiculo,
            veiculoId);

        var handler = CriarHandler(
            veiculoRepository: CriarVeiculoRepository(veiculo),
            pessoaRepository: CriarPessoaRepository(pessoa));

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "O modelo deve ser informado.",
            result.Error);
    }

    // ============================================================
    // DOMAIN EXCEPTION - ANO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ano_For_Invalido()
    {
        var pessoaId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var command = CriarCommand(
            veiculoId: veiculoId,
            pessoaId: pessoaId,
            anoFabricacao: 1800);

        var pessoa = CriarPessoa(pessoaId);

        var veiculo = CriarVeiculo(
            pessoaId,
            "ABC1D23");

        DefinirId(
            veiculo,
            veiculoId);

        var handler = CriarHandler(
            veiculoRepository: CriarVeiculoRepository(veiculo),
            pessoaRepository: CriarPessoaRepository(pessoa));

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Ano do veículo inválido.",
            result.Error);
    }

    // ============================================================
    // EXCEPTION
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao_Ao_Buscar_Veiculo()
    {
        var command = CriarCommand();

        var veiculoRepository =
            new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível atualizar o veículo.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao_Ao_Buscar_Pessoa()
    {
        var command = CriarCommand();

        var veiculo = CriarVeiculo(
            command.PessoaId,
            "ABC1D23");

        DefinirId(
            veiculo,
            command.VeiculoId);

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
            veiculoRepository: CriarVeiculoRepository(veiculo),
            pessoaRepository: pessoaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível atualizar o veículo.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao_Ao_Buscar_Veiculos()
    {
        var command = CriarCommand();

        var pessoa =
            CriarPessoa(command.PessoaId);

        var veiculo =
            CriarVeiculo(
                command.PessoaId,
                "ABC1D23");

        DefinirId(
            veiculo,
            command.VeiculoId);

        var veiculoRepository =
            new Mock<IVeiculoRepository>();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        veiculoRepository
            .Setup(x => x.GetAllAsync(
                includePessoa: true,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: CriarPessoaRepository(pessoa));

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível atualizar o veículo.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao_Ao_Atualizar_Veiculo()
    {
        var command = CriarCommand();

        var pessoa =
            CriarPessoa(command.PessoaId);

        var veiculo =
            CriarVeiculo(
                command.PessoaId,
                "ABC1D23");

        DefinirId(
            veiculo,
            command.VeiculoId);

        var veiculoRepository =
            CriarVeiculoRepository(veiculo);

        veiculoRepository
            .Setup(x => x.UpdateAsync(
                It.IsAny<Veiculo>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            veiculoRepository: veiculoRepository,
            pessoaRepository: CriarPessoaRepository(pessoa));

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível atualizar o veículo.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao_Ao_Salvar()
    {
        var command = CriarCommand();

        var pessoa =
            CriarPessoa(command.PessoaId);

        var veiculo =
            CriarVeiculo(
                command.PessoaId,
                "ABC1D23");

        DefinirId(
            veiculo,
            command.VeiculoId);

        var unitOfWork =
            new Mock<IUnitOfWork>();

        unitOfWork
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            veiculoRepository: CriarVeiculoRepository(veiculo),
            pessoaRepository: CriarPessoaRepository(pessoa),
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível atualizar o veículo.",
            result.Error);
    }

    // ============================================================
    // FACTORY - HANDLER
    // ============================================================

    private static UpdateVeiculoCommandHandler CriarHandler(
        Mock<IVeiculoRepository>? veiculoRepository = null,
        Mock<IRepository<Pessoa>>? pessoaRepository = null,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        return new UpdateVeiculoCommandHandler(
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

            NullLogger<UpdateVeiculoCommandHandler>.Instance);
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
        CriarVeiculoRepository(
            Veiculo veiculo)
    {
        var repository =
            new Mock<IVeiculoRepository>();

        repository
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        repository
            .Setup(x => x.GetAllAsync(
                includePessoa: true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new[]
                {
                    veiculo
                });

        return repository;
    }

    // ============================================================
    // FACTORY - COMMAND
    // ============================================================

    private static UpdateVeiculoCommand CriarCommand(
        Guid? veiculoId = null,
        Guid? pessoaId = null,
        string? placa = "DEF2E34",
        string? marca = "Honda",
        string? modelo = "Civic",
        int anoFabricacao = 2025,
        string? cor = "Prata",
        int hodometro = 25000)
    {
        var command =
            (UpdateVeiculoCommand)
                RuntimeHelpers.GetUninitializedObject(
                    typeof(UpdateVeiculoCommand));

        DefinirPropriedade(
            command,
            nameof(UpdateVeiculoCommand.VeiculoId),
            veiculoId ?? Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(UpdateVeiculoCommand.PessoaId),
            pessoaId ?? Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(UpdateVeiculoCommand.Placa),
            placa!);

        DefinirPropriedade(
            command,
            nameof(UpdateVeiculoCommand.Marca),
            marca!);

        DefinirPropriedade(
            command,
            nameof(UpdateVeiculoCommand.Modelo),
            modelo!);

        DefinirPropriedade(
            command,
            nameof(UpdateVeiculoCommand.AnoFabricacao),
            anoFabricacao);

        DefinirPropriedade(
            command,
            nameof(UpdateVeiculoCommand.Cor),
            cor);

        DefinirPropriedade(
            command,
            nameof(UpdateVeiculoCommand.Hodometro),
            hodometro);

        return command;
    }

    // ============================================================
    // FACTORY - PESSOA
    // ============================================================

    private static Pessoa CriarPessoa(
    Guid? id = null)
    {
        var pessoa =
            new Pessoa(
                "João da Silva",
                new Cpf("52998224725"),
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

        if (id.HasValue)
        {
            DefinirId(
                pessoa,
                id.Value);
        }

        return pessoa;
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
    // REFLECTION - ID
    // ============================================================

    private static void DefinirId(
        object entidade,
        Guid id)
    {
        DefinirPropriedade(
            entidade,
            nameof(Entity.Id),
            id);
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