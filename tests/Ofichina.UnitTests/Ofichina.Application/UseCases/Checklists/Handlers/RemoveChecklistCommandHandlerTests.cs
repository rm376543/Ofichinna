using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Application.UseCases.Checklists.Handlers;
using Ofichina.Contracts.Requests.Checklist;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.Application.UseCases.Checklists.Handlers;

public sealed class RemoveChecklistCommandHandlerTests
{
    // ============================================================  
    // SUCESSO  
    // ============================================================  

    [Fact]
    public async Task Deve_Remover_Checklist_Quando_Dados_Forem_Validos()
    {
        // Arrange  
        var command = CriarCommand();
        var checklist = CriarChecklist();

        var repository = new Mock<IChecklistRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarChecklist(repository, command, checklist);

        var handler = CriarHandler(repository, unitOfWork);

        // Act  
        var result = await handler.HandleAsync(command);

        // Assert  
        Assert.True(result.IsSuccess, result.Error);
        Assert.True(checklist.EstaExcluida());

        repository.Verify(
            x => x.UpdateAsync(checklist, It.IsAny<CancellationToken>()),
            Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    // ============================================================  
    // CHECKLIST NÃO ENCONTRADO  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Checklist_Nao_For_Encontrado()
    {
        // Arrange  
        var command = CriarCommand();

        var repository = new Mock<IChecklistRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        repository
            .Setup(x => x.GetByAgendamentoChecklistIdAsync(
                command.AgendamentoId,
                command.ChecklistId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Checklist?)null);

        var handler = CriarHandler(repository, unitOfWork);

        // Act  
        var result = await handler.HandleAsync(command);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Checklist não encontrado.", result.Error);

        repository.Verify(
            x => x.UpdateAsync(It.IsAny<Checklist>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ============================================================  
    // CHECKLIST JÁ EXCLUÍDO  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Checklist_Ja_Estiver_Excluido()
    {
        // Arrange  
        var command = CriarCommand();
        var checklist = CriarChecklist();
        checklist.Excluir();

        var repository = new Mock<IChecklistRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarChecklist(repository, command, checklist);

        var handler = CriarHandler(repository, unitOfWork);

        // Act  
        var result = await handler.HandleAsync(command);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Checklist já está excluída.", result.Error);

        repository.Verify(
            x => x.UpdateAsync(It.IsAny<Checklist>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ============================================================  
    // CHECKLIST JÁ FINALIZADO  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Checklist_Ja_Estiver_Finalizado()
    {
        // Arrange  
        var command = CriarCommand();
        var checklist = CriarChecklist();
        checklist.Finalizar();

        var repository = new Mock<IChecklistRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarChecklist(repository, command, checklist);

        var handler = CriarHandler(repository, unitOfWork);

        // Act  
        var result = await handler.HandleAsync(command);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Checklist já está finalizado.", result.Error);

        repository.Verify(
            x => x.UpdateAsync(It.IsAny<Checklist>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ============================================================  
    // EXCEÇÃO INESPERADA  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Ocorrer_Excecao()
    {
        // Arrange  
        var command = CriarCommand();

        var repository = new Mock<IChecklistRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        repository
            .Setup(x => x.GetByAgendamentoChecklistIdAsync(
                command.AgendamentoId,
                command.ChecklistId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ThrowsAsync(new InvalidOperationException("Falha inesperada."));

        var handler = CriarHandler(repository, unitOfWork);

        // Act  
        var result = await handler.HandleAsync(command);

        // Assert  
        Assert.False(result.IsSuccess);
        Assert.Equal("Ocorreu um erro.", result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static RemoveChecklistCommand CriarCommand()
    {
        return new RemoveChecklistCommand(new RemoveChecklistRequest
        {
            AgendamentoId = Guid.NewGuid(),
            ChecklistId = Guid.NewGuid()
        });
    }

    private static RemoveChecklistCommandHandler CriarHandler(
        Mock<IChecklistRepository> repository,
        Mock<IUnitOfWork> unitOfWork)
    {
        return new RemoveChecklistCommandHandler(
            repository.Object,
            unitOfWork.Object,
            NullLogger<RemoveChecklistCommandHandler>.Instance);
    }

    private static void ConfigurarChecklist(
        Mock<IChecklistRepository> repository,
        RemoveChecklistCommand command,
        Checklist checklist)
    {
        repository
            .Setup(x => x.GetByAgendamentoChecklistIdAsync(
                command.AgendamentoId,
                command.ChecklistId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(checklist);
    }

    private static Checklist CriarChecklist()
    {
        return new Checklist(
            Guid.NewGuid(),
            "Luzes, freios e pneus",
            "Checklist inicial");
    }
}