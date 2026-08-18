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

namespace Ofichina.UnitTests.Application.ItensServico.Handlers;

public sealed class UpdateItemServicoCommandHandlerTests
{
    // ============================================================
    // ITEM DE SERVIÇO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Item_Nao_Existir()
    {
        var command = CriarCommand();

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true,
                includeRelacionados: true))
            .ReturnsAsync((ItemServico?)null);

        var handler = CriarHandler(
            itemServicoRepository: itemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Item de serviço não encontrado.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Item_Estiver_Excluido()
    {
        var command = CriarCommand();

        var item = CriarItemServico();
        DefinirDeletedAt(item);

        var itemServicoRepository =
            new Mock<IItemServicoRepository>();

        itemServicoRepository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true,
                includeRelacionados: true))
            .ReturnsAsync(item);

        var handler = CriarHandler(
            itemServicoRepository: itemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Item de serviço não encontrado.",
            result.Error);
    }

    // ============================================================
    // ORDEM DE SERVIÇO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_OrdemServico_Nao_Existir()
    {
        var command = CriarCommand();

        var itemServicoRepository =
            CriarItemServicoRepositoryValido(command);

        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServico?)null);

        var handler = CriarHandler(
            ordemServicoRepository: ordemServicoRepository,
            itemServicoRepository: itemServicoRepository);

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

        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var handler = CriarHandler(
            ordemServicoRepository: ordemServicoRepository,
            itemServicoRepository:
                CriarItemServicoRepositoryValido(command));

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
            StatusOrdemServico.Criado);

        var ordemServicoRepository =
            new Mock<IOrdemServicoRepository>();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var handler = CriarHandler(
            ordemServicoRepository: ordemServicoRepository,
            itemServicoRepository:
                CriarItemServicoRepositoryValido(command));

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

        var servicoRepository =
            new Mock<IRepository<Servico>>();

        servicoRepository
            .Setup(x => x.GetByIdAsync(
                command.ServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync((Servico?)null);

        var handler = CriarHandler(
            ordemServicoRepository:
                CriarOrdemServicoRepositoryValido(command),
            itemServicoRepository:
                CriarItemServicoRepositoryValido(command),
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

        var servicoRepository =
            new Mock<IRepository<Servico>>();

        servicoRepository
            .Setup(x => x.GetByIdAsync(
                command.ServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(servico);

        var handler = CriarHandler(
            ordemServicoRepository:
                CriarOrdemServicoRepositoryValido(command),
            itemServicoRepository:
                CriarItemServicoRepositoryValido(command),
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

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync((Peca?)null);

        var handler = CriarHandler(
            ordemServicoRepository:
                CriarOrdemServicoRepositoryValido(command),
            itemServicoRepository:
                CriarItemServicoRepositoryValido(command),
            servicoRepository:
                CriarServicoRepositoryValido(command),
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

        var pecaRepository =
            new Mock<IRepository<Peca>>();

        pecaRepository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(peca);

        var handler = CriarHandler(
            ordemServicoRepository:
                CriarOrdemServicoRepositoryValido(command),
            itemServicoRepository:
                CriarItemServicoRepositoryValido(command),
            servicoRepository:
                CriarServicoRepositoryValido(command),
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
    // DUPLICIDADE
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Item_Duplicado_Existir()
    {
        var command = CriarCommand();

        var duplicado = CriarItemServico();

        DefinirPropriedade(
            duplicado,
            nameof(ItemServico.Id),
            Guid.NewGuid());

        var itemServicoRepository =
            CriarItemServicoRepositoryValido(
                command,
                duplicado: duplicado);

        var handler = CriarHandler(
            ordemServicoRepository:
                CriarOrdemServicoRepositoryValido(command),
            itemServicoRepository: itemServicoRepository,
            servicoRepository:
                CriarServicoRepositoryValido(command),
            pecaRepository:
                CriarPecaRepositoryValido(command));

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Já existe um item de serviço com este serviço e esta peça na ordem.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Permitir_Atualizacao_Quando_Duplicado_For_O_Proprio_Item()
    {
        var command = CriarCommand();

        var duplicado = CriarItemServico();

        DefinirPropriedade(
            duplicado,
            nameof(ItemServico.Id),
            command.ItemServicoId);

        var itemServicoRepository =
            CriarItemServicoRepositoryValido(
                command,
                duplicado: duplicado);

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            ordemServicoRepository:
                CriarOrdemServicoRepositoryValido(command),
            itemServicoRepository: itemServicoRepository,
            servicoRepository:
                CriarServicoRepositoryValido(command),
            pecaRepository:
                CriarPecaRepositoryValido(command),
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Deve_Permitir_Atualizacao_Quando_Duplicado_Estiver_Excluido()
    {
        var command = CriarCommand();

        var duplicado = CriarItemServico();

        DefinirPropriedade(
            duplicado,
            nameof(ItemServico.Id),
            Guid.NewGuid());

        DefinirDeletedAt(duplicado);

        var itemServicoRepository =
            CriarItemServicoRepositoryValido(
                command,
                duplicado: duplicado);

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            ordemServicoRepository:
                CriarOrdemServicoRepositoryValido(command),
            itemServicoRepository: itemServicoRepository,
            servicoRepository:
                CriarServicoRepositoryValido(command),
            pecaRepository:
                CriarPecaRepositoryValido(command),
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // SUCESSO
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Atualizar_Item_Com_Sucesso()
    {
        var command = CriarCommand();

        var itemServicoRepository =
            CriarItemServicoRepositoryValido(command);

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var handler = CriarHandler(
            ordemServicoRepository:
                CriarOrdemServicoRepositoryValido(command),
            itemServicoRepository: itemServicoRepository,
            servicoRepository:
                CriarServicoRepositoryValido(command),
            pecaRepository:
                CriarPecaRepositoryValido(command),
            unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        itemServicoRepository.Verify(
            x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true,
                includeRelacionados: true),
            Times.Once);

        itemServicoRepository.Verify(
            x => x.GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(
                command.OrdemServicoId,
                command.ServicoId,
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true),
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
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true,
                includeRelacionados: true))
            .ThrowsAsync(
                new DomainException(
                    "Erro de domínio."));

        var handler = CriarHandler(
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
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true,
                includeRelacionados: true))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            itemServicoRepository: itemServicoRepository);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não foi possível atualizar o item de serviço.",
            result.Error);
    }

    // ============================================================
    // FACTORY
    // ============================================================

    private static UpdateItemServicoCommandHandler CriarHandler(
        Mock<IOrdemServicoRepository>? ordemServicoRepository = null,
        Mock<IItemServicoRepository>? itemServicoRepository = null,
        Mock<IRepository<Servico>>? servicoRepository = null,
        Mock<IRepository<Peca>>? pecaRepository = null,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        return new UpdateItemServicoCommandHandler(
            (ordemServicoRepository ??
                new Mock<IOrdemServicoRepository>()).Object,

            (itemServicoRepository ??
                new Mock<IItemServicoRepository>()).Object,

            (servicoRepository ??
                new Mock<IRepository<Servico>>()).Object,

            (pecaRepository ??
                new Mock<IRepository<Peca>>()).Object,

            (unitOfWork ??
                new Mock<IUnitOfWork>()).Object,

            Microsoft.Extensions.Logging.Abstractions
                .NullLogger<UpdateItemServicoCommandHandler>
                .Instance);
    }

    // ============================================================
    // REPOSITORIES
    // ============================================================

    private static Mock<IOrdemServicoRepository>
        CriarOrdemServicoRepositoryValido(
            UpdateItemServicoCommand command)
    {
        var repository =
            new Mock<IOrdemServicoRepository>();

        var ordemServico =
            CriarOrdemServico();

        DefinirStatus(
            ordemServico,
            StatusOrdemServico.Recebida);

        repository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        return repository;
    }

    private static Mock<IItemServicoRepository>
        CriarItemServicoRepositoryValido(
            UpdateItemServicoCommand command,
            ItemServico? duplicado = null)
    {
        var repository =
            new Mock<IItemServicoRepository>();

        var item =
            CriarItemServico();

        DefinirPropriedade(
            item,
            nameof(ItemServico.Id),
            command.ItemServicoId);

        repository
            .Setup(x => x.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                It.IsAny<CancellationToken>(),
                tracking: true,
                includeRelacionados: true))
            .ReturnsAsync(item);

        repository
            .Setup(x => x.GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(
                command.OrdemServicoId,
                command.ServicoId,
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(duplicado);

        return repository;
    }

    private static Mock<IRepository<Servico>>
        CriarServicoRepositoryValido(
            UpdateItemServicoCommand command)
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

    private static Mock<IRepository<Peca>>
        CriarPecaRepositoryValido(
            UpdateItemServicoCommand command)
    {
        var repository =
            new Mock<IRepository<Peca>>();

        repository
            .Setup(x => x.GetByIdAsync(
                command.PecaId,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(CriarPeca());

        return repository;
    }

    // ============================================================
    // COMMAND
    // ============================================================

    private static UpdateItemServicoCommand CriarCommand()
    {
        var command =
            (UpdateItemServicoCommand)
                FormatterServices.GetUninitializedObject(
                    typeof(UpdateItemServicoCommand));

        DefinirPropriedade(
            command,
            nameof(UpdateItemServicoCommand.OrdemServicoId),
            Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(UpdateItemServicoCommand.ItemServicoId),
            Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(UpdateItemServicoCommand.ServicoId),
            Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(UpdateItemServicoCommand.PecaId),
            Guid.NewGuid());

        DefinirPropriedade(
            command,
            nameof(UpdateItemServicoCommand.Quantidade),
            1);

        return command;
    }

    // ============================================================
    // ENTITIES
    // ============================================================

    private static OrdemServico CriarOrdemServico()
    {
        return (OrdemServico)
            FormatterServices.GetUninitializedObject(
                typeof(OrdemServico));
    }

    private static ItemServico CriarItemServico()
    {
        return (ItemServico)
            FormatterServices.GetUninitializedObject(
                typeof(ItemServico));
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