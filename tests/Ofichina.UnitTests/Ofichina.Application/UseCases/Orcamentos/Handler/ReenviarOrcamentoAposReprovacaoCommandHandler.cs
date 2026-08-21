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

public sealed class ReenviarOrcamentoAposReprovacaoCommandHandlerTests
{
    // ============================================================  
    // SUCESSO  
    // ============================================================  

    [Fact]
    public async Task Deve_Reenviar_Orcamento_Quando_Dados_Forem_Validos()
    {
        // Arrange  
        var orcamento = CriarOrcamentoReprovado();
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
                It.Is<HistoricoStatus>(h =>
                    ObterValorPropriedade<Guid>(
                        h,
                        "OrcamentoId") == orcamento.Id),
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
    // ORÇAMENTO NÃO ENCONTRADO  
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
            x => x.GetByIdAsync(
                command.Id,
                It.IsAny<CancellationToken>(),
                tracking: true),
            Times.Once);

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
    // ORÇAMENTO EXCLUÍDO  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Orcamento_Estiver_Excluido()
    {
        // Arrange  
        var orcamento = CriarOrcamentoReprovado();

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
    public async Task Deve_Retornar_Falha_Quando_Reenviar_Orcamento_Lancar_DomainException()
    {
        // Arrange  
        // Orçamento recém-criado nasce em "Criado", status inválido para reenvio.  
        var orcamento = CriarOrcamento();
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
    // EXCEPTION INESPERADA - GET  
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
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Falha inesperada."));

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
            "Não foi possível reenviar o orçamento.",
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
    // EXCEPTION INESPERADA - UPDATE  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Atualizacao_Lancar_Excecao()
    {
        // Arrange  
        var orcamento = CriarOrcamentoReprovado();
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
                    "Falha ao atualizar orçamento."));

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
            "Não foi possível reenviar o orçamento.",
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
    // EXCEPTION INESPERADA - HISTÓRICO  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Historico_Lancar_Excecao()
    {
        // Arrange  
        var orcamento = CriarOrcamentoReprovado();
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
                    "Falha ao salvar histórico."));

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
            "Não foi possível reenviar o orçamento.",
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
    // EXCEPTION INESPERADA - UNIT OF WORK  
    // ============================================================  

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_SaveChanges_Lancar_Excecao()
    {
        // Arrange  
        var orcamento = CriarOrcamentoReprovado();
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
                    "Falha ao persistir alterações."));

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
            "Não foi possível reenviar o orçamento.",
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
            Times.Once);
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static ReenviarOrcamentoAposReprovacaoCommand CriarCommand(
        Guid? id = null)
    {
        return new ReenviarOrcamentoAposReprovacaoCommand(
            id ?? Guid.NewGuid());
    }

    private static ReenviarOrcamentoAposReprovacaoCommandHandler CriarHandler(
        Mock<IOrcamentoRepository> orcamentoRepository,
        Mock<IRepository<HistoricoStatus>> historicoStatusRepository,
        Mock<IUserService> usuarioAtualService,
        Mock<IUnitOfWork> unitOfWork)
    {
        return new ReenviarOrcamentoAposReprovacaoCommandHandler(
            orcamentoRepository.Object,
            historicoStatusRepository.Object,
            usuarioAtualService.Object,
            unitOfWork.Object,
            NullLogger<ReenviarOrcamentoAposReprovacaoCommandHandler>.Instance);
    }

    private static void ConfigurarOrcamento(
        Mock<IOrcamentoRepository> repository,
        Orcamento orcamento)
    {
        repository
            .Setup(x => x.GetByIdAsync(
                orcamento.Id,
                It.IsAny<CancellationToken>(),
                tracking: true))
            .ReturnsAsync(orcamento);
    }

    private static Orcamento CriarOrcamentoReprovado()
    {
        var orcamento = CriarOrcamento();

        ConfigurarStatusReprovado(orcamento);

        return orcamento;
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

    // ============================================================  
    // STATUS  
    // ============================================================  

    private static void ConfigurarStatusReprovado(
        Orcamento orcamento)
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
            $"A propriedade Status deveria ser um enum, mas é {tipo.FullName}.");

        var nomes = Enum.GetNames(tipo);

        var candidatos = new[]
        {
            "Reprovado"
        };

        var nome = candidatos.FirstOrDefault(
            x => nomes.Contains(
                x,
                StringComparer.OrdinalIgnoreCase));

        Assert.False(
            string.IsNullOrWhiteSpace(nome),
            $"Não foi encontrado o status Reprovado. Status disponíveis: {string.Join(", ", nomes)}");

        var valor = Enum.Parse(tipo, nome!);

        propriedade.SetValue(
            orcamento,
            valor);
    }

    // ============================================================  
    // REFLECTION HELPERS  
    // ============================================================  

    private static T ObterValorPropriedade<T>(
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

        metodo!.Invoke(entidade, null);
    }
}