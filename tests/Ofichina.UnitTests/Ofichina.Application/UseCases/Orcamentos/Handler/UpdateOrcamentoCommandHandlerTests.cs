using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Application.UseCases.Orcamentos.Handlers;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using System.Reflection;

namespace Ofichina.UnitTests.Application.UseCases.Orcamentos.Handlers;

public sealed class UpdateOrcamentoCommandHandlerTests
{
    // ============================================================
    // SUCESSO
    // ============================================================

    [Fact]
    public async Task Deve_Atualizar_Orcamento_Quando_Todos_Os_Dados_Forem_Validos()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo();
        var mecanico = CriarPessoa();
        var consultor = CriarPessoa();

        var pessoaId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();
        var mecanicoId = Guid.NewGuid();
        var consultorId = Guid.NewGuid();
        var dataValidade = new DateOnly(2026, 12, 31);
        var observacoes = "Orçamento atualizado pelo cliente.";

        var command = CriarCommand(
            orcamento.Id,
            pessoaId,
            veiculoId,
            mecanicoId,
            consultorId,
            dataValidade,
            observacoes);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(orcamentoRepository, orcamento);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                mecanicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(mecanico);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                consultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(consultor);

        ConfigurarVeiculo(
            veiculoRepository,
            veiculoId,
            veiculo);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess, result.Error);

        Assert.Equal(pessoaId, orcamento.PessoaId);
        Assert.Equal(veiculoId, orcamento.VeiculoId);
        Assert.Equal(mecanicoId, orcamento.MecanicoId);
        Assert.Equal(consultorId, orcamento.ConsultorId);
        Assert.Equal(
            dataValidade.ToDateTime(TimeOnly.MinValue),
            orcamento.DataValidade);
        Assert.Equal(observacoes, orcamento.Observacoes);

        orcamentoRepository.Verify(
            x => x.GetByIdAsync(
                orcamento.Id,
                true,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                pessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                mecanicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                consultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        veiculoRepository.Verify(
            x => x.GetByIdAsync(
                veiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                orcamento,
                It.IsAny<CancellationToken>()),
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
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        orcamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrcamentoId,
                true,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Orcamento?)null);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Orçamento não encontrado.",
            result.Error);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        veiculoRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Orcamento_Estiver_Excluido()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        MarcarComoExcluido(orcamento);

        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Orçamento não encontrado.",
            result.Error);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        veiculoRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // PESSOA
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Pessoa_Nao_For_Encontrada()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Pessoa?)null);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Pessoa não encontrada.",
            result.Error);

        veiculoRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Pessoa_Estiver_Excluida()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        var pessoa = CriarPessoa();

        MarcarComoExcluido(pessoa);

        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Pessoa não encontrada.",
            result.Error);

        veiculoRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // VEÍCULO
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Veiculo_Nao_For_Encontrado()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        var pessoa = CriarPessoa();
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa);

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Veiculo?)null);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Veículo não encontrado.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Veiculo_Estiver_Excluido()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo();

        MarcarComoExcluido(veiculo);

        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa);

        ConfigurarVeiculo(
            veiculoRepository,
            command.VeiculoId,
            veiculo);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Veículo não encontrado.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // MECÂNICO
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Mecanico_Nao_For_Encontrado()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo();
        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        ConfigurarVeiculo(
            veiculoRepository,
            command.VeiculoId,
            veiculo);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.MecanicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Pessoa?)null);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Mecânico do diagnóstico não encontrado.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Mecanico_Estiver_Excluido()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo();
        var mecanico = CriarPessoa();

        MarcarComoExcluido(mecanico);

        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        ConfigurarVeiculo(
            veiculoRepository,
            command.VeiculoId,
            veiculo);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.MecanicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(mecanico);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Mecânico do diagnóstico não encontrado.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // CONSULTOR
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Consultor_Nao_For_Encontrado()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo();
        var mecanico = CriarPessoa();

        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        ConfigurarVeiculo(
            veiculoRepository,
            command.VeiculoId,
            veiculo);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.MecanicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(mecanico);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Pessoa?)null);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Consultor não encontrado.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Consultor_Estiver_Excluido()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo();
        var mecanico = CriarPessoa();
        var consultor = CriarPessoa();

        MarcarComoExcluido(consultor);

        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        ConfigurarVeiculo(
            veiculoRepository,
            command.VeiculoId,
            veiculo);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.MecanicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(mecanico);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(consultor);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Consultor não encontrado.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // DOMAIN EXCEPTION
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Atualizacao_Lancar_DomainException()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo();
        var mecanico = CriarPessoa();
        var consultor = CriarPessoa();

        var command = CriarCommand(
            orcamento.Id,
            pessoaId: Guid.Empty);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa);

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(veiculo);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.MecanicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(mecanico);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(consultor);

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Pessoa obrigatória.",
            result.Error);

        orcamentoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Orcamento>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // EXCEPTION INESPERADA
    // ============================================================

    [Fact]
    public async Task Deve_Retornar_Falha_Quando_Ocorrer_Erro_Inesperado()
    {
        // Arrange
        var orcamento = CriarOrcamentoValido();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo();
        var mecanico = CriarPessoa();
        var consultor = CriarPessoa();

        var command = CriarCommand(orcamento.Id);

        var orcamentoRepository = new Mock<IOrcamentoRepository>();
        var pessoaRepository = new Mock<IRepository<Pessoa>>();
        var veiculoRepository = new Mock<IRepository<Veiculo>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        ConfigurarOrcamento(
            orcamentoRepository,
            orcamento);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.MecanicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(mecanico);

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(consultor);

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(veiculo);

        orcamentoRepository
            .Setup(x => x.UpdateAsync(
                orcamento,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Falha inesperada de infraestrutura."));

        var handler = CriarHandler(
            orcamentoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não foi possível atualizar o orçamento.",
            result.Error);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private static UpdateOrcamentoCommand CriarCommand(
        Guid? orcamentoId = null,
        Guid? pessoaId = null,
        Guid? veiculoId = null,
        Guid? mecanicoId = null,
        Guid? consultorId = null,
        DateOnly? dataValidade = null,
        string? observacoes = "Observação padrão.")
    {
        return new UpdateOrcamentoCommand(
            new UpdateOrcamentoRequest
            {
                OrcamentoId = orcamentoId ?? Guid.NewGuid(),
                PessoaId = pessoaId ?? Guid.NewGuid(),
                VeiculoId = veiculoId ?? Guid.NewGuid(),
                MecanicoId = mecanicoId ?? Guid.NewGuid(),
                ConsultorId = consultorId ?? Guid.NewGuid(),
                DataValidade = dataValidade ?? new DateOnly(2026, 12, 31),
                Observacoes = observacoes
            });
    }

    private static Orcamento CriarOrcamentoValido()
        => new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            0m,
            null);

    private static Pessoa CriarPessoa()
        => (Pessoa)Activator.CreateInstance(
            typeof(Pessoa),
            BindingFlags.Instance |
            BindingFlags.NonPublic,
            binder: null,
            args: null,
            culture: null)!;

    private static Veiculo CriarVeiculo()
        => (Veiculo)Activator.CreateInstance(
            typeof(Veiculo),
            BindingFlags.Instance |
            BindingFlags.NonPublic,
            binder: null,
            args: null,
            culture: null)!;

    private static UpdateOrcamentoCommandHandler CriarHandler(
        Mock<IOrcamentoRepository> orcamentoRepository,
        Mock<IRepository<Pessoa>> pessoaRepository,
        Mock<IRepository<Veiculo>> veiculoRepository,
        Mock<IUnitOfWork> unitOfWork)
    {
        return new UpdateOrcamentoCommandHandler(
            orcamentoRepository.Object,
            pessoaRepository.Object,
            veiculoRepository.Object,
            unitOfWork.Object,
            NullLogger<UpdateOrcamentoCommandHandler>.Instance);
    }

    private static void ConfigurarOrcamento(
        Mock<IOrcamentoRepository> repository,
        Orcamento orcamento)
    {
        repository
            .Setup(x => x.GetByIdAsync(
                orcamento.Id,
                true,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(orcamento);
    }

    private static void ConfigurarVeiculo(
        Mock<IRepository<Veiculo>> repository,
        Guid veiculoId,
        Veiculo veiculo)
    {
        repository
            .Setup(x => x.GetByIdAsync(
                veiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(veiculo);
    }

    private static void MarcarComoExcluido(object entidade)
    {
        var metodo = entidade
            .GetType()
            .GetMethod(
                "Excluir",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic)
            ?? entidade
                .GetType()
                .BaseType?
                .GetMethod(
                    "Excluir",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

        Assert.NotNull(metodo);

        metodo!.Invoke(entidade, null);
    }
}