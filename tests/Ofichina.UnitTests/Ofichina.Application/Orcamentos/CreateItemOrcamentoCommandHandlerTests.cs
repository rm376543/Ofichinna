using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Application.UseCases.ItensServico.Handlers;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Ofichina.UnitTests.Application.Orcamentos;

public sealed class CreateItemOrcamentoCommandHandlerTests
{
    // ============================================================
    // ORÇAMENTO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Orcamento_Nao_Existir()
    {
        var command = CriarCommand();

        var orcamentoRepository = new Mock<IOrcamentoRepository>();

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrcamentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Orcamento?)null);

        var handler = CriarHandler(
            orcamentoRepository: orcamentoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Orçamento não encontrado.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Orcamento_Estiver_Excluido()
    {
        var command = CriarCommand();

        var orcamento = CriarOrcamento();
        DefinirDeletedAt(orcamento);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrcamentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(orcamento);

        var handler = CriarHandler(
            orcamentoRepository: orcamentoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Orçamento não encontrado.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Orcamento_Nao_Estiver_Em_Diagnostico()
    {
        var command = CriarCommand();

        var orcamento = CriarOrcamento();
        DefinirStatus(
            orcamento,
            StatusOrcamento.AguardandoAprovacao);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrcamentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(orcamento);

        var handler = CriarHandler(
            orcamentoRepository: orcamentoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não é possível alterar itens nesta etapa do orçamento.",
            result.Error);
    }

    // ============================================================
    // SERVIÇO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Servico_Nao_Existir()
    {
        var command = CriarCommand();

        var orcamentoRepository = CriarOrcamentoRepositoryValido(
            command);

        var servicoRepository = new Mock<IRepository<Servico>>();

        servicoRepository
            .Setup(x => x.GetByIdAsync(
                command.ServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync((Servico?)null);

        var handler = CriarHandler(
            orcamentoRepository: orcamentoRepository,
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

        var orcamentoRepository = CriarOrcamentoRepositoryValido(
            command);

        var servicoRepository = new Mock<IRepository<Servico>>();

        servicoRepository
            .Setup(x => x.GetByIdAsync(
                command.ServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(servico);

        var handler = CriarHandler(
            orcamentoRepository: orcamentoRepository,
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
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_PecaRepository_Nao_Estiver_Disponivel()
    {
        var command = CriarCommand(
            pecaId: Guid.NewGuid());

        var handler = CriarHandler(
            orcamentoRepository: CriarOrcamentoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
            pecaRepository: null);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Repositório de peças não está disponível.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Peca_Nao_Existir()
    {
        var command = CriarCommand(
            pecaId: Guid.NewGuid());

        var pecaRepository = new Mock<IRepository<Peca>>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId!.Value,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync((Peca?)null);

        var handler = CriarHandler(
            orcamentoRepository: CriarOrcamentoRepositoryValido(command),
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
        var command = CriarCommand(
            pecaId: Guid.NewGuid());

        var peca = CriarPeca();
        DefinirDeletedAt(peca);

        var pecaRepository = new Mock<IRepository<Peca>>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId!.Value,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(peca);

        var handler = CriarHandler(
            orcamentoRepository: CriarOrcamentoRepositoryValido(command),
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

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoServicoPecaIdAsync(
                command.OrcamentoId,
                command.ServicoId,
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(itemExistente);

        var handler = CriarHandler(
            orcamentoRepository: CriarOrcamentoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
            itemServicoRepository: itemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Já existe um item de serviço com este serviço e esta peça vinculado ao orcamento.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Permitir_Criacao_Quando_Item_Existente_Estiver_Excluido()
    {
        var command = CriarCommand();

        var itemExistente = CriarItemServico();
        DefinirDeletedAt(itemExistente);

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoServicoPecaIdAsync(
                command.OrcamentoId,
                command.ServicoId,
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(itemExistente);

        var handler = CriarHandler(
            orcamentoRepository: CriarOrcamentoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
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
    public async Task HandleAsync_Deve_Criar_Item_Com_Sucesso_Sem_Peca()
    {
        var command = CriarCommand();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            orcamentoRepository: CriarOrcamentoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
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

    [Fact]
    public async Task HandleAsync_Deve_Criar_Item_Com_Sucesso_Com_Peca()
    {
        var command = CriarCommand(
            pecaId: Guid.NewGuid());

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId!.Value,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(CriarPeca());

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            orcamentoRepository: CriarOrcamentoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
            pecaRepository: pecaRepository,
            itemServicoRepository: itemServicoRepository,
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        pecaRepository.Verify(
            x => x.GetByIdAsync(
                command.PecaId!.Value,
                It.IsAny<CancellationToken>(),
                tracking: true),
            Times.Once);

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

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoServicoPecaIdAsync(
                command.OrcamentoId,
                command.ServicoId,
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ThrowsAsync(
                new DomainException(
                    "Erro de domínio."));

        var handler = CriarHandler(
            orcamentoRepository: CriarOrcamentoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
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

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        itemServicoRepository
            .Setup(x => x.GetByOrcamentoServicoPecaIdAsync(
                command.OrcamentoId,
                command.ServicoId,
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            orcamentoRepository: CriarOrcamentoRepositoryValido(command),
            servicoRepository: CriarServicoRepositoryValido(command),
            itemServicoRepository: itemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Ocorreu um erro ao criar o item de serviço.",
            result.Error);
    }

    // ============================================================
    // FACTORY
    // ============================================================

    private static CreateItemOrcamentoCommandHandler CriarHandler(
        Mock<IOrcamentoRepository>? orcamentoRepository = null,
        Mock<IItemServicoRepository>? itemServicoRepository = null,
        Mock<IRepository<Servico>>? servicoRepository = null,
        Mock<IRepository<Peca>>? pecaRepository = null,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        return new CreateItemOrcamentoCommandHandler(
            (orcamentoRepository ?? new Mock<IOrcamentoRepository>()).Object,
            (itemServicoRepository ?? new Mock<IItemServicoRepository>()).Object,
            (servicoRepository ?? new Mock<IRepository<Servico>>()).Object,
            pecaRepository?.Object,
            (unitOfWork ?? new Mock<IUnitOfWork>()).Object,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<
                CreateItemOrcamentoCommandHandler>.Instance);
    }

    // ============================================================
    // REPOSITORIES
    // ============================================================

    private static Mock<IOrcamentoRepository>
        CriarOrcamentoRepositoryValido(
            CreateItemOrcamentoCommand command)
    {
        var repository = new Mock<IOrcamentoRepository>();

        var orcamento = CriarOrcamento();

        DefinirStatus(
            orcamento,
            StatusOrcamento.EmDiagnostico);

        repository
            .Setup(x => x.GetByIdAsync(
                command.OrcamentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(orcamento);

        return repository;
    }

    private static Mock<IRepository<Servico>>
        CriarServicoRepositoryValido(
            CreateItemOrcamentoCommand command)
    {
        var repository =
            new Mock<IRepository<Servico>>();

        repository
            .Setup(x => x.GetByIdAsync(
                command.ServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(CriarServico());

        return repository;
    }

    // ============================================================
    // COMMAND
    // ============================================================

    private static CreateItemOrcamentoCommand CriarCommand(
        Guid? pecaId = null)
    {
        var command =
            (CreateItemOrcamentoCommand)
                FormatterServices.GetUninitializedObject(
                    typeof(CreateItemOrcamentoCommand));

        DefinirPropriedade(
            command,
            nameof(CreateItemOrcamentoCommand.OrcamentoId),
            Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(CreateItemOrcamentoCommand.ServicoId),
            Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(CreateItemOrcamentoCommand.PecaId),
            pecaId);

        DefinirPropriedade(
            command,
            nameof(CreateItemOrcamentoCommand.Quantidade),
            1);

        return command;
    }

    // ============================================================
    // ENTITIES
    // ============================================================

    private static Orcamento CriarOrcamento()
    {
        return (Orcamento)
            FormatterServices.GetUninitializedObject(
                typeof(Orcamento));
    }

    private static Servico CriarServico()
    {
        return (Servico)
            FormatterServices.GetUninitializedObject(
                typeof(Servico));
    }

    private static Peca CriarPeca()
    {
        return (Peca)
            FormatterServices.GetUninitializedObject(
                typeof(Peca));
    }

    private static ItemServico CriarItemServico()
    {
        return (ItemServico)
            FormatterServices.GetUninitializedObject(
                typeof(ItemServico));
    }

    // ============================================================
    // REFLECTION HELPERS
    // ============================================================

    private static void DefinirStatus(
        Orcamento orcamento,
        StatusOrcamento status)
    {
        DefinirPropriedade(
            orcamento,
            nameof(Orcamento.Status),
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