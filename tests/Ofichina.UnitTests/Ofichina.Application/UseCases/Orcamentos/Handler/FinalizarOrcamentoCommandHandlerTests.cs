using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Application.UseCases.Orcamentos.Handlers;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using System.Reflection;

namespace Ofichina.UnitTests.Application.UseCases.Orcamentos.Handlers;

public sealed class FinalizarOrcamentoCommandHandlerTests
{
    // ============================================================
    // SUCESSO
    // ============================================================

    [Fact]
    public async Task Deve_Finalizar_Orcamento_Quando_Dados_Forem_Validos()
    {
        // Arrange
        var orcamento = CriarOrcamentoParaFinalizacao();
        var command = CriarCommand(orcamento.Id);
        var usuarioId = Guid.NewGuid();

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository =
            new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        usuarioAtualService
            .Setup(x => x.ObterUsuarioId())
            .Returns(usuarioId);

        var handler = CriarHandler(
            orcamentoRepository,
            historicoStatusRepository,
            usuarioAtualService,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess, result.Error);

        orcamentoRepository.Verify(
            x => x.GetByIdAsync(
                command.Id,
                includeItens: true,
                It.IsAny<CancellationToken>(),
                tracking: true),
            Times.Once);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                orcamento,
                It.IsAny<CancellationToken>()),
            Times.Once);

        historicoStatusRepository.Verify(
            x => x.AddAsync(
                It.Is<HistoricoStatus>(x =>
                    ObterPropriedade<Guid>(x, "OrcamentoId")
                    == orcamento.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);

        usuarioAtualService.Verify(
            x => x.ObterUsuarioId(),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // ORÇAMENTO
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Orcamento_Nao_For_Encontrado()
    {
        // Arrange
        var command = CriarCommand();

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository =
            new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.Id,
                includeItens: true,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync((Orcamento?)null);

        var handler = CriarHandler(
            orcamentoRepository,
            historicoStatusRepository,
            usuarioAtualService,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Orçamento não encontrado.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        historicoStatusRepository.Verify(
            x => x.AddAsync(
                It.IsAny<HistoricoStatus>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        usuarioAtualService.Verify(
            x => x.ObterUsuarioId(),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Orcamento_Estiver_Excluido()
    {
        // Arrange
        var orcamento = CriarOrcamentoParaFinalizacao();

        Excluir(orcamento);

        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository =
            new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        var handler = CriarHandler(
            orcamentoRepository,
            historicoStatusRepository,
            usuarioAtualService,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Orçamento não encontrado.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        historicoStatusRepository.Verify(
            x => x.AddAsync(
                It.IsAny<HistoricoStatus>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        usuarioAtualService.Verify(
            x => x.ObterUsuarioId(),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // DOMAIN EXCEPTION
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_FinalizarDiagnostico_Lancar_DomainException()
    {
        // Arrange
        var orcamento = CriarOrcamentoComStatusInvalido();
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository =
            new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        var handler = CriarHandler(
            orcamentoRepository,
            historicoStatusRepository,
            usuarioAtualService,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Error));

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        historicoStatusRepository.Verify(
            x => x.AddAsync(
                It.IsAny<HistoricoStatus>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // EXCEPTION - GET
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Repositorio_Lancar_Excecao_Ao_Buscar_Orcamento()
    {
        // Arrange
        var command = CriarCommand();

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository =
            new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.Id,
                includeItens: true,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            orcamentoRepository,
            historicoStatusRepository,
            usuarioAtualService,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não foi possível finalizar o orçamento.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        historicoStatusRepository.Verify(
            x => x.AddAsync(
                It.IsAny<HistoricoStatus>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // EXCEPTION - UPDATE
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Update_Lancar_Excecao()
    {
        // Arrange
        var orcamento = CriarOrcamentoParaFinalizacao();
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository =
            new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        usuarioAtualService
            .Setup(x => x.ObterUsuarioId())
            .Returns(Guid.NewGuid());

        orcamentoRepository
            .Setup(x => x.UpdateAsync(
                orcamento,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro ao atualizar."));

        var handler = CriarHandler(
            orcamentoRepository,
            historicoStatusRepository,
            usuarioAtualService,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não foi possível finalizar o orçamento.",
            result.Error);

        historicoStatusRepository.Verify(
            x => x.AddAsync(
                It.IsAny<HistoricoStatus>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // EXCEPTION - HISTÓRICO
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Historico_Lancar_Excecao()
    {
        // Arrange
        var orcamento = CriarOrcamentoParaFinalizacao();
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository =
            new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        usuarioAtualService
            .Setup(x => x.ObterUsuarioId())
            .Returns(Guid.NewGuid());

        historicoStatusRepository
            .Setup(x => x.AddAsync(
                It.IsAny<HistoricoStatus>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro ao adicionar histórico."));

        var handler = CriarHandler(
            orcamentoRepository,
            historicoStatusRepository,
            usuarioAtualService,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não foi possível finalizar o orçamento.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                orcamento,
                It.IsAny<CancellationToken>()),
            Times.Once);

        historicoStatusRepository.Verify(
            x => x.AddAsync(
                It.IsAny<HistoricoStatus>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // EXCEPTION - SAVE CHANGES
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_SaveChanges_Lancar_Excecao()
    {
        // Arrange
        var orcamento = CriarOrcamentoParaFinalizacao();
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var historicoStatusRepository =
            new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        usuarioAtualService
            .Setup(x => x.ObterUsuarioId())
            .Returns(Guid.NewGuid());

        unitOfWork
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro ao salvar alterações."));

        var handler = CriarHandler(
            orcamentoRepository,
            historicoStatusRepository,
            usuarioAtualService,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não foi possível finalizar o orçamento.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                orcamento,
                It.IsAny<CancellationToken>()),
            Times.Once);

        historicoStatusRepository.Verify(
            x => x.AddAsync(
                It.IsAny<HistoricoStatus>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        usuarioAtualService.Verify(
            x => x.ObterUsuarioId(),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private static FinalizarOrcamentoCommand CriarCommand(
        Guid? id = null)
    {
        return new FinalizarOrcamentoCommand(
            id ?? Guid.NewGuid());
    }

    private static FinalizarOrcamentoCommandHandler CriarHandler(
        Mock<IOrcamentoRepository> orcamentoRepository,
        Mock<IRepository<HistoricoStatus>> historicoStatusRepository,
        Mock<IUserService> usuarioAtualService,
        Mock<IUnitOfWork> unitOfWork)
    {
        return new FinalizarOrcamentoCommandHandler(
            orcamentoRepository.Object,
            historicoStatusRepository.Object,
            usuarioAtualService.Object,
            unitOfWork.Object,
            NullLogger<FinalizarOrcamentoCommandHandler>.Instance);
    }

    private static void ConfigurarOrcamento(
        Mock<IOrcamentoRepository> repository,
        Orcamento orcamento)
    {
        repository
            .Setup(x => x.GetByIdAsync(
                orcamento.Id,
                includeItens: true,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(orcamento);
    }

    private static Orcamento CriarOrcamento()
    {
        return new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            0m,
            null);
    }

    private static Orcamento CriarOrcamentoParaFinalizacao()
    {
        var orcamento = CriarOrcamento();

        ConfigurarStatus(
            orcamento,
            new[]
            {
                "EmDiagnostico",
                "AguardandoDiagnostico",
                "EmElaboracao",
                "AguardandoFinalizacao",
                "EmAndamento",
                "Rascunho"
            });

        var status = ObterStatusOrcamento(orcamento);

        orcamento.AdicionarServico(
            Guid.NewGuid(),
            null,
            1,
            status);

        return orcamento;
    }

    private static StatusOrcamento ObterStatusOrcamento(Orcamento orcamento)
    {
        var propriedade = typeof(Orcamento)
            .GetProperty(
                "Status",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        Assert.NotNull(propriedade);

        var valor = propriedade!.GetValue(orcamento);

        Assert.NotNull(valor);

        return (StatusOrcamento)valor;
    }

    private static Orcamento CriarOrcamentoComStatusInvalido()
    {
        var orcamento = CriarOrcamento();

        ConfigurarStatus(
            orcamento,
            new[]
            {
                "AguardandoAprovacao",
                "Aprovado",
                "Enviado",
                "Finalizado",
                "Cancelado",
                "Recusado"
            });

        return orcamento;
    }

    // ============================================================
    // STATUS
    // ============================================================

    private static void ConfigurarStatus(
        Orcamento orcamento,
        IEnumerable<string> candidatos)
    {
        var propriedade = typeof(Orcamento)
            .GetProperty(
                "Status",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        Assert.NotNull(propriedade);

        var tipo = Nullable.GetUnderlyingType(
            propriedade!.PropertyType)
            ?? propriedade.PropertyType;

        Assert.True(
            tipo.IsEnum,
            $"A propriedade Status deveria ser enum. Tipo encontrado: {tipo.FullName}");

        var nomes = Enum.GetNames(tipo);

        var nomeEncontrado = candidatos.FirstOrDefault(
            candidato => nomes.Any(
                nome => string.Equals(
                    nome,
                    candidato,
                    StringComparison.OrdinalIgnoreCase)));

        Assert.False(
            string.IsNullOrWhiteSpace(nomeEncontrado),
            $"Nenhum dos status esperados foi encontrado. " +
            $"Status disponíveis: {string.Join(", ", nomes)}");

        var status = Enum.Parse(
            tipo,
            nomeEncontrado!,
            ignoreCase: true);

        propriedade.SetValue(
            orcamento,
            status);
    }

    // ============================================================
    // EXCLUSÃO
    // ============================================================

    private static void Excluir(object entidade)
    {
        var metodo = entidade
            .GetType()
            .GetMethod(
                "Excluir",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        if (metodo is null)
        {
            metodo = entidade
                .GetType()
                .BaseType?
                .GetMethod(
                    "Excluir",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);
        }

        Assert.NotNull(metodo);

        metodo!.Invoke(
            entidade,
            null);
    }

    // ============================================================
    // REFLECTION
    // ============================================================

    private static T ObterPropriedade<T>(
        object entidade,
        string nome)
    {
        var propriedade = entidade
            .GetType()
            .GetProperty(
                nome,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        Assert.NotNull(propriedade);

        var valor = propriedade!.GetValue(entidade);

        Assert.NotNull(valor);

        return (T)valor!;
    }
}