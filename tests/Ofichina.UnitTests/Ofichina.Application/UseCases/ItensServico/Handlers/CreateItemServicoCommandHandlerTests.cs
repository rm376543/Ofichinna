using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Application.UseCases.ItensServico.Handlers;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;
using Ofichina.UnitTests.TestInfrastructure;
using Ofichina.UnitTests.TestInfrastructure.Builders;
using TestMocks = Ofichina.UnitTests.TestInfrastructure.Mocks;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ofichina.UnitTests.Application.UseCases.ItensServico.Handlers;

public sealed class CreateItemServicoCommandHandlerTests
{
    // ============================================================
    // ORDEM DE SERVIÇO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_OrdemServico_Nao_Existir()
    {
        var command = CriarCommand();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServico?)null);

        var handler = CriarHandler(
            ordemServicoRepository: ordemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Ordem de serviço não encontrada.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_OrdemServico_Estiver_Excluida()
    {
        var command = CriarCommand();

        var ordemServico = CriarOrdemServico();
        DefinirDeletedAt(ordemServico);

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var handler = CriarHandler(
            ordemServicoRepository: ordemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Ordem de serviço não encontrada.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_OrdemServico_Nao_Estiver_Recebida()
    {
        var command = CriarCommand();

        var ordemServico = CriarOrdemServico();
        DefinirStatus(
            ordemServico,
            StatusOrdemServico.EmExecucao);

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var handler = CriarHandler(
            ordemServicoRepository: ordemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não é possível alterar itens nesta etapa da OS.",
            result.Error);
    }

    // ============================================================
    // SERVIÇO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Servico_Nao_Existir()
    {
        var command = CriarCommand();

        var ordemServicoRepository = CriarOrdemServicoRepositoryValido(command);

        var servicoRepository = new Mock<IRepository<Servico>>();

        servicoRepository
            .Setup(x => x.GetByIdAsync(
                command.ServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync((Servico?)null);

        var handler = CriarHandler(
            ordemServicoRepository: ordemServicoRepository,
            servicoRepository: servicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Serviço não encontrado.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Servico_Estiver_Excluido()
    {
        var command = CriarCommand();

        var servico = CriarServico();
        DefinirDeletedAt(servico);

        var ordemServicoRepository = CriarOrdemServicoRepositoryValido(command);

        var servicoRepository = new Mock<IRepository<Servico>>();

        servicoRepository
            .Setup(x => x.GetByIdAsync(
                command.ServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(servico);

        var handler = CriarHandler(
            ordemServicoRepository: ordemServicoRepository,
            servicoRepository: servicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Serviço não encontrado.",
            result.Error);
    }

    // ============================================================
    // PEÇA
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Peca_Nao_Existir()
    {
        var command = CriarCommand();

        var pecaRepository = new Mock<IRepository<Peca>>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync((Peca?)null);

        var handler = CriarHandler(
            ordemServicoRepository: CriarOrdemServicoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
            pecaRepository: pecaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Peça não encontrada.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Peca_Estiver_Excluida()
    {
        var command = CriarCommand();

        var peca = CriarPeca();
        DefinirDeletedAt(peca);

        var pecaRepository = new Mock<IRepository<Peca>>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(peca);

        var handler = CriarHandler(
            ordemServicoRepository: CriarOrdemServicoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
            pecaRepository: pecaRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Peça não encontrada.",
            result.Error);
    }

    // ============================================================
    // ITEM EXISTENTE
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Item_Ja_Existir()
    {
        var command = CriarCommand();

        var itemExistente = CriarItemServico();

        var itemServicoRepository = new Mock<IItemServicoRepository>();

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(
                command.OrdemServicoId,
                command.ServicoId,
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(itemExistente);

        var handler = CriarHandler(
            ordemServicoRepository: CriarOrdemServicoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
            pecaRepository: CriarPecaRepositoryValido(command),
            itemServicoRepository: itemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Já existe um item de serviço com este serviço e esta peça na ordem.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Permitir_Criacao_Quando_Item_Existente_Estiver_Excluido()
    {
        var command = CriarCommand();

        var itemExistente = CriarItemServico();
        DefinirDeletedAt(itemExistente);

        var itemServicoRepository = new Mock<IItemServicoRepository>();

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(
                command.OrdemServicoId,
                command.ServicoId,
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(itemExistente);

        var handler = CriarHandler(
            ordemServicoRepository: CriarOrdemServicoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
            pecaRepository: CriarPecaRepositoryValido(command),
            itemServicoRepository: itemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    // ============================================================
    // SUCESSO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Criar_Item_Com_Sucesso_Quando_Item_Nao_Existir()
    {
        var command = CriarCommand();

        var itemServicoRepository = new Mock<IItemServicoRepository>();

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(
                command.OrdemServicoId,
                command.ServicoId,
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync((ItemServico?)null);

        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            ordemServicoRepository: CriarOrdemServicoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
            pecaRepository: CriarPecaRepositoryValido(command),
            itemServicoRepository: itemServicoRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        itemServicoRepository.Verify(
            x => x.AddAsync(
                It.IsAny<ItemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // DOMAIN EXCEPTION
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_DomainException()
    {
        var command = CriarCommand();

        var itemServicoRepository = new Mock<IItemServicoRepository>();

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(
                command.OrdemServicoId,
                command.ServicoId,
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ThrowsAsync(
                new DomainException(
                    "Erro de domínio."));

        var handler = CriarHandler(
            ordemServicoRepository: CriarOrdemServicoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
            pecaRepository: CriarPecaRepositoryValido(command),
            itemServicoRepository: itemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Erro de domínio.",
            result.Error);
    }

    // ============================================================
    // EXCEPTION
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao()
    {
        var command = CriarCommand();

        var itemServicoRepository = new Mock<IItemServicoRepository>();

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(
                command.OrdemServicoId,
                command.ServicoId,
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            ordemServicoRepository: CriarOrdemServicoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
            pecaRepository: CriarPecaRepositoryValido(command),
            itemServicoRepository: itemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não foi possível criar o item de serviço.",
            result.Error);
    }

    // ============================================================
    // FACTORY
    // ============================================================

    private static CreateItemServicoCommandHandler CriarHandler(
        Mock<IOrdemServicoRepository>? ordemServicoRepository = null,
        Mock<IItemServicoRepository>? itemServicoRepository = null,
        Mock<IRepository<Servico>>? servicoRepository = null,
        Mock<IRepository<Peca>>? pecaRepository = null,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        return new CreateItemServicoCommandHandler(
            (ordemServicoRepository ?? new Mock<IOrdemServicoRepository>()).Object,
            (itemServicoRepository ?? new Mock<IItemServicoRepository>()).Object,
            (servicoRepository ?? new Mock<IRepository<Servico>>()).Object,
            (pecaRepository ?? new Mock<IRepository<Peca>>()).Object,
            (unitOfWork ?? new Mock<IUnitOfWork>()).Object,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<
                CreateItemServicoCommandHandler>.Instance);
    }

    // ============================================================
    // REPOSITORIES
    // ============================================================

    private static Mock<IOrdemServicoRepository>
        CriarOrdemServicoRepositoryValido(
            CreateItemServicoCommand command)
    {
        var ordemServico = TestDataFactory.OrdensServico.Builder()
            .ComId(command.OrdemServicoId)
            .Criada()
            .Build();

        var repository = TestMocks.MockFactory.OrdemServicoRepository.ComGetById(ordemServico);

        return repository;
    }

    private static Mock<IRepository<Servico>>
        CriarServicoRepositoryValido(
            CreateItemServicoCommand command)
    {
        var servico = TestDataFactory.Servicos.Criar();
        ReflectionHelpers.DefinirId(servico, command.ServicoId);
        var repository = TestMocks.MockFactory.Repositorio<Servico>.ComGetById(servico);
        return repository;
    }

    private static Mock<IRepository<Peca>>
        CriarPecaRepositoryValido(
            CreateItemServicoCommand command)
    {
        var peca = TestDataFactory.Pecas.Criar();
        ReflectionHelpers.DefinirId(peca, command.PecaId);
        var repository = TestMocks.MockFactory.Repositorio<Peca>.ComGetById(peca);
        return repository;
    }

    // ============================================================
    // COMMAND
    // ============================================================

    private static CreateItemServicoCommand CriarCommand()
    {
        var command =
    (CreateItemServicoCommand)
        RuntimeHelpers.GetUninitializedObject(
            typeof(CreateItemServicoCommand));

        DefinirPropriedade(
            command,
            nameof(CreateItemServicoCommand.OrdemServicoId),
            Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(CreateItemServicoCommand.ServicoId),
            Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(CreateItemServicoCommand.PecaId),
            Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(CreateItemServicoCommand.Quantidade),
            1);

        return command;
    }

    // ============================================================
    // ENTITIES
    // ============================================================

    private static OrdemServico CriarOrdemServico()
    {
        return (OrdemServico)
            RuntimeHelpers.GetUninitializedObject(
            typeof(OrdemServico));
    }

    private static Servico CriarServico()
    {
        return (Servico)
            RuntimeHelpers.GetUninitializedObject(
                typeof(Servico));
    }

    private static Peca CriarPeca()
    {
        return (Peca)
            RuntimeHelpers.GetUninitializedObject(
                typeof(Peca));
    }

    private static ItemServico CriarItemServico()
    {
        return (ItemServico)
            RuntimeHelpers.GetUninitializedObject(
                typeof(ItemServico));
    }

    // ============================================================
    // REFLECTION HELPERS
    // ============================================================

    private static void DefinirStatus(
        OrdemServico ordemServico,
        StatusOrdemServico status)
    {
        DefinirPropriedade(
            ordemServico,
            nameof(OrdemServico.Status),
            status);
    }

    private static void DefinirDeletedAt(
        object entidade)
    {
        DefinirPropriedade(
            entidade,
            "DeletedAt",
            DateTime.UtcNow);
    }

    private static void DefinirPropriedade(
        object objeto,
        string nome,
        object? valor)
    {
        var property = objeto
            .GetType()
            .GetProperty(
                nome,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        if (property is null)
            throw new InvalidOperationException(
                $"A propriedade '{nome}' não foi encontrada em '{objeto.GetType().Name}'.");

        property.SetValue(
            objeto,
            valor);
    }
}