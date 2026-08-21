using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Application.UseCases.Orcamentos.Handlers;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using System.Reflection;

namespace Ofichina.UnitTests.Application.UseCases.Orcamentos.Handlers;

public sealed class IniciarDiagnosticoOrcamentoCommandHandlerTests
{
    // ================= SUCESSO =================  
    [Fact]
    public async Task Deve_Iniciar_Diagnostico_Quando_Dados_Forem_Validos()
    {
        var orcamento = CriarOrcamento();
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(orcamentoRepository, orcamento);
        usuarioAtualService.Setup(x => x.ObterUsuarioId()).Returns(Guid.NewGuid());

        var handler = CriarHandler(orcamentoRepository, historicoStatusRepository, usuarioAtualService, unitOfWork);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess, result.Error);
        orcamentoRepository.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>(), true), Times.Once);
        orcamentoRepository.Verify(x => x.UpdateAsync(orcamento, It.IsAny<CancellationToken>()), Times.Once);
        historicoStatusRepository.Verify(x => x.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()), Times.Once);
        usuarioAtualService.Verify(x => x.ObterUsuarioId(), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    // ================= NÃO ENCONTRADO =================  
    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Orcamento_Nao_For_Encontrado()
    {
        var command = CriarCommand();

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((Orcamento?)null);

        var handler = CriarHandler(orcamentoRepository, historicoStatusRepository, usuarioAtualService, unitOfWork);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Orçamento não encontrado.", result.Error);
        orcamentoRepository.Verify(x => x.UpdateAsync(It.IsAny<Orcamento>(), It.IsAny<CancellationToken>()), Times.Never);
        historicoStatusRepository.Verify(x => x.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()), Times.Never);
        usuarioAtualService.Verify(x => x.ObterUsuarioId(), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ================= EXCLUÍDO =================  
    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Orcamento_Estiver_Excluido()
    {
        var orcamento = CriarOrcamento();
        Excluir(orcamento);
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(orcamentoRepository, orcamento);

        var handler = CriarHandler(orcamentoRepository, historicoStatusRepository, usuarioAtualService, unitOfWork);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Orçamento não encontrado.", result.Error);
        orcamentoRepository.Verify(x => x.UpdateAsync(It.IsAny<Orcamento>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ================= DOMAIN EXCEPTION =================  
    [Fact]
    public async Task Deve_Retornar_Falha_Quando_IniciarDiagnostico_Lancar_DomainException()
    {
        var orcamento = CriarOrcamento();
        orcamento.IniciarDiagnostico(); // move para EmDiagnostico -> segunda chamada lança DomainException  
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(orcamentoRepository, orcamento);

        var handler = CriarHandler(orcamentoRepository, historicoStatusRepository, usuarioAtualService, unitOfWork);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Error));
        orcamentoRepository.Verify(x => x.UpdateAsync(It.IsAny<Orcamento>(), It.IsAny<CancellationToken>()), Times.Never);
        historicoStatusRepository.Verify(x => x.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ================= EXCEPTION - GET =================  
    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Repositorio_Lancar_Excecao_Ao_Buscar()
    {
        var command = CriarCommand();

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>(), true))
            .ThrowsAsync(new InvalidOperationException("Erro inesperado."));

        var handler = CriarHandler(orcamentoRepository, historicoStatusRepository, usuarioAtualService, unitOfWork);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível iniciar o diagnóstico do orçamento.", result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ================= EXCEPTION - UPDATE =================  
    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Update_Lancar_Excecao()
    {
        var orcamento = CriarOrcamento();
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(orcamentoRepository, orcamento);
        orcamentoRepository
            .Setup(x => x.UpdateAsync(orcamento, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Erro ao atualizar."));

        var handler = CriarHandler(orcamentoRepository, historicoStatusRepository, usuarioAtualService, unitOfWork);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível iniciar o diagnóstico do orçamento.", result.Error);
        historicoStatusRepository.Verify(x => x.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ================= EXCEPTION - HISTÓRICO =================  
    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Historico_Lancar_Excecao()
    {
        var orcamento = CriarOrcamento();
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(orcamentoRepository, orcamento);
        usuarioAtualService.Setup(x => x.ObterUsuarioId()).Returns(Guid.NewGuid());
        historicoStatusRepository
            .Setup(x => x.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Erro ao adicionar histórico."));

        var handler = CriarHandler(orcamentoRepository, historicoStatusRepository, usuarioAtualService, unitOfWork);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível iniciar o diagnóstico do orçamento.", result.Error);
        orcamentoRepository.Verify(x => x.UpdateAsync(orcamento, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ================= EXCEPTION - SAVE CHANGES =================  
    [Fact]
    public async Task Deve_Retornar_Falha_Quando_SaveChanges_Lancar_Excecao()
    {
        var orcamento = CriarOrcamento();
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(orcamentoRepository, orcamento);
        usuarioAtualService.Setup(x => x.ObterUsuarioId()).Returns(Guid.NewGuid());
        unitOfWork
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(new InvalidOperationException("Erro ao salvar alterações."));

        var handler = CriarHandler(orcamentoRepository, historicoStatusRepository, usuarioAtualService, unitOfWork);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível iniciar o diagnóstico do orçamento.", result.Error);
        orcamentoRepository.Verify(x => x.UpdateAsync(orcamento, It.IsAny<CancellationToken>()), Times.Once);
        historicoStatusRepository.Verify(x => x.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    // ================= HELPERS =================  
    private static IniciarDiagnosticoOrcamentoCommand CriarCommand(Guid? id = null)
        => new IniciarDiagnosticoOrcamentoCommand(id ?? Guid.NewGuid());

    private static IniciarDiagnosticoOrcamentoCommandHandler CriarHandler(
        Mock<IOrcamentoRepository> orcamentoRepository,
        Mock<IRepository<HistoricoStatus>> historicoStatusRepository,
        Mock<IUserService> usuarioAtualService,
        Mock<IUnitOfWork> unitOfWork)
        => new IniciarDiagnosticoOrcamentoCommandHandler(
            orcamentoRepository.Object,
            historicoStatusRepository.Object,
            usuarioAtualService.Object,
            unitOfWork.Object,
            NullLogger<IniciarDiagnosticoOrcamentoCommandHandler>.Instance);

    private static void ConfigurarOrcamento(Mock<IOrcamentoRepository> repository, Orcamento orcamento)
        => repository
            .Setup(x => x.GetByIdAsync(orcamento.Id, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(orcamento);

    private static Orcamento CriarOrcamento()
        => new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            0m,
            null);

    private static void Excluir(object entidade)
    {
        var metodo = entidade.GetType().GetMethod("Excluir",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? entidade.GetType().BaseType?.GetMethod("Excluir",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(metodo);
        metodo!.Invoke(entidade, null);
    }
}