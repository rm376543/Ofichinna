using Microsoft.Extensions.Logging;
using Moq;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Application.UseCases.Agendamentos.Handlers;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using System.Reflection;

namespace Ofichina.UnitTests.Application.UseCases.Agendamentos.Handlers;

public sealed class CreateAgendamentoCommandHandlerTests
{
    private readonly Mock<IAgendamentoRepository> _agendamentoRepository;
    private readonly Mock<IPessoaRepository> _pessoaRepository;
    private readonly Mock<IVeiculoRepository> _veiculoRepository;
    private readonly Mock<IAgendaConsultorRepository> _agendaConsultorRepository;
    private readonly Mock<IProfileAuthService> _perfilAutorizacaoService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<CreateAgendamentoCommandHandler>> _logger;

    private readonly CreateAgendamentoCommandHandler _handler;

    public CreateAgendamentoCommandHandlerTests()
    {
        _agendamentoRepository = new Mock<IAgendamentoRepository>();
        _pessoaRepository = new Mock<IPessoaRepository>();
        _veiculoRepository = new Mock<IVeiculoRepository>();
        _agendaConsultorRepository = new Mock<IAgendaConsultorRepository>();
        _perfilAutorizacaoService = new Mock<IProfileAuthService>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger<CreateAgendamentoCommandHandler>>();

        _handler = new CreateAgendamentoCommandHandler(
            _agendamentoRepository.Object,
            _pessoaRepository.Object,
            _veiculoRepository.Object,
            _agendaConsultorRepository.Object,
            _perfilAutorizacaoService.Object,
            _unitOfWork.Object,
            _logger.Object);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Pessoa_Nao_Existir()
    {
        // Arrange
        var command = CriarCommand();

        _pessoaRepository
            .Setup(x => x.GetByIdAsync(command.PessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pessoa?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Pessoa não encontrada.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Pessoa_Estiver_Excluida()
    {
        // Arrange
        var command = CriarCommand();

        var pessoa = CriarPessoa();
        MarcarComoExcluida(pessoa);

        _pessoaRepository
            .Setup(x => x.GetByIdAsync(command.PessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Pessoa não encontrada.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Veiculo_Nao_Existir()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();

        _pessoaRepository
            .Setup(x => x.GetByIdAsync(command.PessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Veiculo?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Veículo não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Veiculo_Estiver_Excluido()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);

        MarcarComoExcluida(veiculo);

        _pessoaRepository
            .Setup(x => x.GetByIdAsync(command.PessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Veículo não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Veiculo_Nao_Pertencer_A_Pessoa()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var outraPessoa = CriarPessoa();
        var veiculo = CriarVeiculo(outraPessoa.Id);

        _pessoaRepository
            .Setup(x => x.GetByIdAsync(command.PessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "O veículo informado não pertence ao usuário autenticado.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Slot_Nao_Existir()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);

        _pessoaRepository
            .Setup(x => x.GetByIdAsync(command.PessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        _agendaConsultorRepository
            .Setup(x => x.GetByIdWithConsultorAsync(
                command.AgendaConsultorId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AgendaConsultor?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Slot de disponibilidade não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Slot_Estiver_Excluido()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);
        var slot = CriarAgendaConsultor();

        MarcarComoExcluida(slot);

        _pessoaRepository
            .Setup(x => x.GetByIdAsync(command.PessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        _agendaConsultorRepository
            .Setup(x => x.GetByIdWithConsultorAsync(
                command.AgendaConsultorId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(slot);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Slot de disponibilidade não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Consultor_Nao_Existir()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);

        var slot = CriarAgendaConsultor();
        DefinirConsultor(slot, null);

        ConfigurarDadosIniciais(command, pessoa, veiculo, slot);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Consultor não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Consultor_Estiver_Excluido()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);
        var slot = CriarAgendaConsultor();

        MarcarComoExcluida(slot.Consultor!);

        ConfigurarDadosIniciais(command, pessoa, veiculo, slot);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Consultor não encontrado.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Consultor_Nao_Possuir_Perfil()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);
        var slot = CriarAgendaConsultor();

        ConfigurarDadosIniciais(command, pessoa, veiculo, slot);

        _perfilAutorizacaoService
            .Setup(x => x.PossuiPerfilAsync(
                slot.Consultor!.UsuarioId,
                "CONSULTOR",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "A pessoa informada não possui perfil de consultor.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Slot_Ja_Estiver_Agendado()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);
        var slot = CriarAgendaConsultor();

        ConfigurarDadosIniciais(command, pessoa, veiculo, slot);

        _perfilAutorizacaoService
            .Setup(x => x.PossuiPerfilAsync(
                slot.Consultor!.UsuarioId,
                "CONSULTOR",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var agendamentoExistente = CriarAgendamento(
            command.PessoaId,
            command.AgendaConsultorId,
            command.VeiculoId);

        _agendamentoRepository
            .Setup(x => x.GetAllWithIncludesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { agendamentoExistente });

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Já existe um agendamento para este horário/consultor.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Veiculo_Ja_Estiver_Agendado_No_Mesmo_Horario()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);
        var slot = CriarAgendaConsultor();

        ConfigurarDadosIniciais(command, pessoa, veiculo, slot);

        _perfilAutorizacaoService
            .Setup(x => x.PossuiPerfilAsync(
                slot.Consultor!.UsuarioId,
                "CONSULTOR",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var outroSlot = CriarAgendaConsultor();

        DefinirDiaDisponibilidade(outroSlot, slot.DiaDisponibilidadeId);
        DefinirHorarioDisponibilidade(outroSlot, slot.HorarioDisponibilidadeId);

        var agendamentoExistente = CriarAgendamento(
            command.PessoaId,
            outroSlot.Id,
            command.VeiculoId);

        DefinirAgendaConsultor(
            agendamentoExistente,
            outroSlot);

        _agendamentoRepository
            .Setup(x => x.GetAllWithIncludesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { agendamentoExistente });

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Já existe um agendamento para este veículo neste horário.",
            result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Criar_Agendamento_Com_Sucesso()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);
        var slot = CriarAgendaConsultor();

        ConfigurarDadosIniciais(command, pessoa, veiculo, slot);

        _perfilAutorizacaoService
            .Setup(x => x.PossuiPerfilAsync(
                slot.Consultor!.UsuarioId,
                "CONSULTOR",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _agendamentoRepository
            .Setup(x => x.GetAllWithIncludesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Agendamento>());

        _agendamentoRepository
            .Setup(x => x.AddAsync(
                It.IsAny<Agendamento>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);

        _agendamentoRepository.Verify(
            x => x.AddAsync(
                It.Is<Agendamento>(a =>
                    a.ClientePessoaId == command.PessoaId &&
                    a.VeiculoId == command.VeiculoId &&
                    a.AgendaConsultorId == command.AgendaConsultorId &&
                    a.Hodometro == command.Hodometro &&
                    a.Descricao == command.Descricao),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_DomainException_For_Lancada()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);
        var slot = CriarAgendaConsultor();

        ConfigurarDadosIniciais(command, pessoa, veiculo, slot);

        _perfilAutorizacaoService
            .Setup(x => x.PossuiPerfilAsync(
                slot.Consultor!.UsuarioId,
                "CONSULTOR",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _agendamentoRepository
            .Setup(x => x.GetAllWithIncludesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Agendamento>());

        _agendamentoRepository
            .Setup(x => x.AddAsync(
                It.IsAny<Agendamento>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(CriarDomainException());

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Erro de domínio.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Exception_For_Lancada()
    {
        // Arrange
        var command = CriarCommand();
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);
        var slot = CriarAgendaConsultor();

        ConfigurarDadosIniciais(command, pessoa, veiculo, slot);

        _perfilAutorizacaoService
            .Setup(x => x.PossuiPerfilAsync(
                slot.Consultor!.UsuarioId,
                "CONSULTOR",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _agendamentoRepository
            .Setup(x => x.GetAllWithIncludesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Agendamento>());

        _agendamentoRepository
            .Setup(x => x.AddAsync(
                It.IsAny<Agendamento>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Erro inesperado"));

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não foi possível criar o agendamento.",
            result.Error);
    }

    #region Helpers

    private static CreateAgendamentoCommand CriarCommand()
    {
        return new CreateAgendamentoCommand(
            pessoaId: Guid.NewGuid(),
            agendaConsultorId: Guid.NewGuid(),
            veiculoId: Guid.NewGuid(),
            hodometro: 10000,
            descricao: "Revisão preventiva");
    }

    private void ConfigurarDadosIniciais(
        CreateAgendamentoCommand command,
        Pessoa pessoa,
        Veiculo veiculo,
        AgendaConsultor slot)
    {
        _pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        _agendaConsultorRepository
            .Setup(x => x.GetByIdWithConsultorAsync(
                command.AgendaConsultorId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(slot);
    }

    private static Pessoa CriarPessoa()
    {
        var pessoa = (Pessoa)Activator.CreateInstance(
            typeof(Pessoa),
            nonPublic: true)!;

        DefinirId(pessoa, Guid.NewGuid());

        return pessoa;
    }

    private static Veiculo CriarVeiculo(Guid pessoaId)
    {
        var veiculo = (Veiculo)Activator.CreateInstance(
            typeof(Veiculo),
            nonPublic: true)!;

        DefinirId(veiculo, Guid.NewGuid());
        DefinirPropriedade(veiculo, nameof(Veiculo.PessoaId), pessoaId);

        return veiculo;
    }

    private static AgendaConsultor CriarAgendaConsultor()
    {
        var slot = (AgendaConsultor)Activator.CreateInstance(
            typeof(AgendaConsultor),
            nonPublic: true)!;

        DefinirId(slot, Guid.NewGuid());

        DefinirPropriedade(
            slot,
            nameof(AgendaConsultor.DiaDisponibilidadeId),
            Guid.NewGuid());

        DefinirPropriedade(
            slot,
            nameof(AgendaConsultor.HorarioDisponibilidadeId),
            Guid.NewGuid());

        var consultor = CriarConsultor();

        DefinirPropriedade(
            slot,
            nameof(AgendaConsultor.Consultor),
            consultor);

        return slot;
    }

    private static Pessoa CriarConsultor()
    {
        var consultor = (Pessoa)Activator.CreateInstance(
            typeof(Pessoa),
            nonPublic: true)!;

        DefinirId(consultor, Guid.NewGuid());

        DefinirPropriedade(
            consultor,
            nameof(Pessoa.UsuarioId),
            Guid.NewGuid());

        return consultor;
    }

    private static Agendamento CriarAgendamento(
        Guid pessoaId,
        Guid agendaConsultorId,
        Guid veiculoId)
    {
        return new Agendamento(
            pessoaId,
            agendaConsultorId,
            veiculoId,
            10000,
            "Revisão");
    }

    private static void DefinirAgendaConsultor(
        Agendamento agendamento,
        AgendaConsultor agendaConsultor)
    {
        DefinirPropriedade(
            agendamento,
            nameof(Agendamento.AgendaConsultor),
            agendaConsultor);
    }

    private static void DefinirConsultor(
        AgendaConsultor slot,
        Pessoa? consultor)
    {
        DefinirPropriedade(
            slot,
            nameof(AgendaConsultor.Consultor),
            consultor);
    }

    private static void DefinirDiaDisponibilidade(
        AgendaConsultor slot,
        Guid id)
    {
        DefinirPropriedade(
            slot,
            nameof(AgendaConsultor.DiaDisponibilidadeId),
            id);
    }

    private static void DefinirHorarioDisponibilidade(
        AgendaConsultor slot,
        Guid id)
    {
        DefinirPropriedade(
            slot,
            nameof(AgendaConsultor.HorarioDisponibilidadeId),
            id);
    }

    private static void DefinirId(object entidade, Guid id)
    {
        DefinirPropriedade(entidade, "Id", id);
    }

    private static void DefinirPropriedade(
        object objeto,
        string propriedade,
        object? valor)
    {
        var property = objeto
            .GetType()
            .GetProperty(
                propriedade,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        property?.SetValue(objeto, valor);
    }

    private static void MarcarComoExcluida(object entidade)
    {
        var propriedade = entidade
            .GetType()
            .GetProperty(
                "DeletedAt",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        propriedade?.SetValue(entidade, DateTime.UtcNow);
    }

    private static DomainException CriarDomainException()
    {
        return (DomainException)Activator.CreateInstance(
            typeof(DomainException),
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic,
            binder: null,
            args: new object[] { "Erro de domínio." },
            culture: null)!;
    }

    #endregion
}