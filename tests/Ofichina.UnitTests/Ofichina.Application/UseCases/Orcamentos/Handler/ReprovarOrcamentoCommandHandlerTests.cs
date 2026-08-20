using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Application.UseCases.Orcamentos.Handlers;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using System.Reflection;

namespace Ofichina.UnitTests.Application.UseCases.Orcamentos.Handlers;

public sealed class ReprovarOrcamentoCommandHandlerTests
{
    [Fact]
    public async Task Deve_Reprovar_Orcamento_Quando_Motivo_For_Informado()
    {
        var orcamento = CriarOrcamentoAguardandoAprovacao();
        var usuarioId = Guid.NewGuid();

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var motivoRecusaRepository = new Mock<IRepository<MotivoRecusaOrcamento>>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        orcamentoRepository.Setup(x => x.GetByIdAsync(orcamento.Id, It.IsAny<CancellationToken>(), true)).ReturnsAsync(orcamento);
        usuarioAtualService.Setup(x => x.ObterUsuarioId()).Returns(usuarioId);

        var handler = new ReprovarOrcamentoCommandHandler(
            orcamentoRepository.Object,
            motivoRecusaRepository.Object,
            historicoStatusRepository.Object,
            usuarioAtualService.Object,
            unitOfWork.Object,
            NullLogger<ReprovarOrcamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new ReprovarOrcamentoCommand(new ReprovarOrcamentoRequest(orcamento.Id, "Cliente desistiu do serviço")));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(StatusOrcamento.Reprovado, orcamento.Status);
        orcamentoRepository.Verify(x => x.UpdateAsync(orcamento, It.IsAny<CancellationToken>()), Times.Once);
        motivoRecusaRepository.Verify(x => x.AddAsync(
            It.Is<MotivoRecusaOrcamento>(m => m != null),
            It.IsAny<CancellationToken>()), Times.Once);
        historicoStatusRepository.Verify(x => x.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Deve_Reprovar_Orcamento_Quando_Motivo_Nao_For_Informado(string? motivo)
    {
        var orcamento = CriarOrcamentoAguardandoAprovacao();

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var motivoRecusaRepository = new Mock<IRepository<MotivoRecusaOrcamento>>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        orcamentoRepository.Setup(x => x.GetByIdAsync(orcamento.Id, It.IsAny<CancellationToken>(), true)).ReturnsAsync(orcamento);
        usuarioAtualService.Setup(x => x.ObterUsuarioId()).Returns(Guid.NewGuid());

        var handler = new ReprovarOrcamentoCommandHandler(
            orcamentoRepository.Object,
            motivoRecusaRepository.Object,
            historicoStatusRepository.Object,
            usuarioAtualService.Object,
            unitOfWork.Object,
            NullLogger<ReprovarOrcamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new ReprovarOrcamentoCommand(new ReprovarOrcamentoRequest(orcamento.Id, motivo)));

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(StatusOrcamento.Reprovado, orcamento.Status);
        motivoRecusaRepository.Verify(x => x.AddAsync(It.IsAny<MotivoRecusaOrcamento>(), It.IsAny<CancellationToken>()), Times.Never);
        historicoStatusRepository.Verify(x => x.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Deve_Recusar_Quando_Orcamento_Nao_For_Encontrado()
    {
        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var motivoRecusaRepository = new Mock<IRepository<MotivoRecusaOrcamento>>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        orcamentoRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), true)).ReturnsAsync((Orcamento?)null);

        var handler = new ReprovarOrcamentoCommandHandler(
            orcamentoRepository.Object,
            motivoRecusaRepository.Object,
            historicoStatusRepository.Object,
            usuarioAtualService.Object,
            unitOfWork.Object,
            NullLogger<ReprovarOrcamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new ReprovarOrcamentoCommand(new ReprovarOrcamentoRequest(Guid.NewGuid(), null)));

        Assert.False(result.IsSuccess);
        Assert.Equal("Orçamento não encontrado.", result.Error);
        orcamentoRepository.Verify(x => x.UpdateAsync(It.IsAny<Orcamento>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Deve_Recusar_Quando_Orcamento_Estiver_Excluido()
    {
        var orcamento = CriarOrcamentoAguardandoAprovacao();
        MarcarComoExcluido(orcamento);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var motivoRecusaRepository = new Mock<IRepository<MotivoRecusaOrcamento>>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        orcamentoRepository.Setup(x => x.GetByIdAsync(orcamento.Id, It.IsAny<CancellationToken>(), true)).ReturnsAsync(orcamento);

        var handler = new ReprovarOrcamentoCommandHandler(
            orcamentoRepository.Object,
            motivoRecusaRepository.Object,
            historicoStatusRepository.Object,
            usuarioAtualService.Object,
            unitOfWork.Object,
            NullLogger<ReprovarOrcamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new ReprovarOrcamentoCommand(new ReprovarOrcamentoRequest(orcamento.Id, null)));

        Assert.False(result.IsSuccess);
        Assert.Equal("Orçamento não encontrado.", result.Error);
        orcamentoRepository.Verify(x => x.UpdateAsync(It.IsAny<Orcamento>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Deve_Recusar_Quando_Orcamento_Nao_Estiver_No_Status_Aguardando_Aprovacao()
    {
        // Orçamento recém-criado permanece no status "Criado", o que faz
        // Orcamento.Reprovar() lançar DomainException.
        var orcamento = CriarOrcamentoValido();

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var motivoRecusaRepository = new Mock<IRepository<MotivoRecusaOrcamento>>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        orcamentoRepository.Setup(x => x.GetByIdAsync(orcamento.Id, It.IsAny<CancellationToken>(), true)).ReturnsAsync(orcamento);

        var handler = new ReprovarOrcamentoCommandHandler(
            orcamentoRepository.Object,
            motivoRecusaRepository.Object,
            historicoStatusRepository.Object,
            usuarioAtualService.Object,
            unitOfWork.Object,
            NullLogger<ReprovarOrcamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new ReprovarOrcamentoCommand(new ReprovarOrcamentoRequest(orcamento.Id, "Motivo qualquer")));

        Assert.False(result.IsSuccess);
        Assert.Equal("O orçamento precisa estar no status AguardandoAprovacao.", result.Error);
        orcamentoRepository.Verify(x => x.UpdateAsync(It.IsAny<Orcamento>(), It.IsAny<CancellationToken>()), Times.Never);
        motivoRecusaRepository.Verify(x => x.AddAsync(It.IsAny<MotivoRecusaOrcamento>(), It.IsAny<CancellationToken>()), Times.Never);
        historicoStatusRepository.Verify(x => x.AddAsync(It.IsAny<HistoricoStatus>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Deve_Recusar_Quando_Ocorrer_Erro_Inesperado()
    {
        var orcamento = CriarOrcamentoAguardandoAprovacao();

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var motivoRecusaRepository = new Mock<IRepository<MotivoRecusaOrcamento>>();
        var historicoStatusRepository = new Mock<IRepository<HistoricoStatus>>();
        var usuarioAtualService = new Mock<IUserService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        orcamentoRepository.Setup(x => x.GetByIdAsync(orcamento.Id, It.IsAny<CancellationToken>(), true)).ReturnsAsync(orcamento);
        usuarioAtualService.Setup(x => x.ObterUsuarioId()).Returns(Guid.NewGuid());
        unitOfWork.Setup(x => x.SaveChangesAsync()).ThrowsAsync(new InvalidOperationException("Falha inesperada de infraestrutura."));

        var handler = new ReprovarOrcamentoCommandHandler(
            orcamentoRepository.Object,
            motivoRecusaRepository.Object,
            historicoStatusRepository.Object,
            usuarioAtualService.Object,
            unitOfWork.Object,
            NullLogger<ReprovarOrcamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new ReprovarOrcamentoCommand(new ReprovarOrcamentoRequest(orcamento.Id, null)));

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível reprovar o orçamento.", result.Error);
    }

    private static Orcamento CriarOrcamentoValido()
        => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 0m, null);

    private static Orcamento CriarOrcamentoAguardandoAprovacao()
    {
        var orcamento = CriarOrcamentoValido();
        DefinirPropriedade(orcamento, nameof(Orcamento.Status), StatusOrcamento.AguardandoAprovacao);
        return orcamento;
    }

    private static void MarcarComoExcluido(Orcamento orcamento)
    {
        var metodo = typeof(Orcamento).GetMethod("Excluir", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? typeof(Orcamento).BaseType?.GetMethod("Excluir", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(metodo);
        metodo!.Invoke(orcamento, null);
    }

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
    }
}