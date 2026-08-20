using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Application.UseCases.Agendamentos.Handlers;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using System.Reflection;

namespace Ofichina.UnitTests.Application.UseCases.Agendamentos.Handlers;

public sealed class CreateAgendamentoCommandHandlerTests
{
    [Fact]
    public async Task Deve_Criar_Agendamento_Quando_Todas_As_Validacoes_Passarem()
    {
        var cliente = CriarPessoaCliente();
        var veiculo = CriarVeiculo(cliente.Id);
        var consultor = CriarPessoaConsultor();
        var slot = CriarSlot(consultor);

        var agendamentoRepository = new Mock<IAgendamentoRepository>();
        var pessoaRepository = new Mock<IPessoaRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var agendaConsultorRepository = new Mock<IAgendaConsultorRepository>();
        var perfilAuthService = new Mock<IProfileAuthService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pessoaRepository.Setup(x => x.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        veiculoRepository.Setup(x => x.GetByIdAsync(veiculo.Id, true, It.IsAny<CancellationToken>())).ReturnsAsync(veiculo);
        agendaConsultorRepository.Setup(x => x.GetByIdWithConsultorAsync(slot.Id, It.IsAny<CancellationToken>())).ReturnsAsync(slot);
        perfilAuthService.Setup(x => x.PossuiPerfilAsync(consultor.UsuarioId, "CONSULTOR", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        agendamentoRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var handler = new CreateAgendamentoCommandHandler(
            agendamentoRepository.Object,
            pessoaRepository.Object,
            veiculoRepository.Object,
            agendaConsultorRepository.Object,
            perfilAuthService.Object,
            unitOfWork.Object,
            NullLogger<CreateAgendamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreateAgendamentoCommand(cliente.Id, slot.Id, veiculo.Id, 55220, "  Revisão preventiva  "));

        Assert.True(result.IsSuccess, result.Error);
        agendamentoRepository.Verify(x => x.AddAsync(It.Is<Agendamento>(a =>
            a.ClientePessoaId == cliente.Id &&
            a.AgendaConsultorId == slot.Id &&
            a.VeiculoId == veiculo.Id &&
            a.Hodometro == 55220 &&
            a.Descricao == "Revisão preventiva"), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Deve_Recusar_Quando_Pessoa_Nao_For_Encontrada()
    {
        var agendamentoRepository = new Mock<IAgendamentoRepository>();
        var pessoaRepository = new Mock<IPessoaRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var agendaConsultorRepository = new Mock<IAgendaConsultorRepository>();
        var perfilAuthService = new Mock<IProfileAuthService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pessoaRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Pessoa?)null);

        var handler = new CreateAgendamentoCommandHandler(
            agendamentoRepository.Object,
            pessoaRepository.Object,
            veiculoRepository.Object,
            agendaConsultorRepository.Object,
            perfilAuthService.Object,
            unitOfWork.Object,
            NullLogger<CreateAgendamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreateAgendamentoCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1000, null));

        Assert.False(result.IsSuccess);
        Assert.Equal("Pessoa não encontrada.", result.Error);
        agendamentoRepository.Verify(x => x.AddAsync(It.IsAny<Agendamento>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Deve_Recusar_Quando_Veiculo_Nao_Pertencer_Ao_Cliente()
    {
        var cliente = CriarPessoaCliente();
        var outroCliente = CriarPessoaCliente();
        var veiculo = CriarVeiculo(outroCliente.Id);

        var agendamentoRepository = new Mock<IAgendamentoRepository>();
        var pessoaRepository = new Mock<IPessoaRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var agendaConsultorRepository = new Mock<IAgendaConsultorRepository>();
        var perfilAuthService = new Mock<IProfileAuthService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pessoaRepository.Setup(x => x.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        veiculoRepository.Setup(x => x.GetByIdAsync(veiculo.Id, true, It.IsAny<CancellationToken>())).ReturnsAsync(veiculo);

        var handler = new CreateAgendamentoCommandHandler(
            agendamentoRepository.Object,
            pessoaRepository.Object,
            veiculoRepository.Object,
            agendaConsultorRepository.Object,
            perfilAuthService.Object,
            unitOfWork.Object,
            NullLogger<CreateAgendamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreateAgendamentoCommand(cliente.Id, Guid.NewGuid(), veiculo.Id, 1000, null));

        Assert.False(result.IsSuccess);
        Assert.Equal("O veículo informado não pertence ao usuário autenticado.", result.Error);
        agendamentoRepository.Verify(x => x.AddAsync(It.IsAny<Agendamento>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Deve_Recusar_Quando_Consultor_Nao_Possuir_Perfil_Consultor()
    {
        var cliente = CriarPessoaCliente();
        var veiculo = CriarVeiculo(cliente.Id);
        var consultor = CriarPessoaConsultor();
        var slot = CriarSlot(consultor);

        var agendamentoRepository = new Mock<IAgendamentoRepository>();
        var pessoaRepository = new Mock<IPessoaRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var agendaConsultorRepository = new Mock<IAgendaConsultorRepository>();
        var perfilAuthService = new Mock<IProfileAuthService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pessoaRepository.Setup(x => x.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        veiculoRepository.Setup(x => x.GetByIdAsync(veiculo.Id, true, It.IsAny<CancellationToken>())).ReturnsAsync(veiculo);
        agendaConsultorRepository.Setup(x => x.GetByIdWithConsultorAsync(slot.Id, It.IsAny<CancellationToken>())).ReturnsAsync(slot);
        perfilAuthService.Setup(x => x.PossuiPerfilAsync(consultor.UsuarioId, "CONSULTOR", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new CreateAgendamentoCommandHandler(
            agendamentoRepository.Object,
            pessoaRepository.Object,
            veiculoRepository.Object,
            agendaConsultorRepository.Object,
            perfilAuthService.Object,
            unitOfWork.Object,
            NullLogger<CreateAgendamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreateAgendamentoCommand(cliente.Id, slot.Id, veiculo.Id, 1000, null));

        Assert.False(result.IsSuccess);
        Assert.Equal("A pessoa informada não possui perfil de consultor.", result.Error);
        agendamentoRepository.Verify(x => x.AddAsync(It.IsAny<Agendamento>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Deve_Recusar_Quando_O_Hodometro_For_Negativo()
    {
        var cliente = CriarPessoaCliente();
        var veiculo = CriarVeiculo(cliente.Id);
        var consultor = CriarPessoaConsultor();
        var slot = CriarSlot(consultor);

        var agendamentoRepository = new Mock<IAgendamentoRepository>();
        var pessoaRepository = new Mock<IPessoaRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var agendaConsultorRepository = new Mock<IAgendaConsultorRepository>();
        var perfilAuthService = new Mock<IProfileAuthService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        pessoaRepository.Setup(x => x.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        veiculoRepository.Setup(x => x.GetByIdAsync(veiculo.Id, true, It.IsAny<CancellationToken>())).ReturnsAsync(veiculo);
        agendaConsultorRepository.Setup(x => x.GetByIdWithConsultorAsync(slot.Id, It.IsAny<CancellationToken>())).ReturnsAsync(slot);
        perfilAuthService.Setup(x => x.PossuiPerfilAsync(consultor.UsuarioId, "CONSULTOR", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        agendamentoRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var handler = new CreateAgendamentoCommandHandler(
            agendamentoRepository.Object,
            pessoaRepository.Object,
            veiculoRepository.Object,
            agendaConsultorRepository.Object,
            perfilAuthService.Object,
            unitOfWork.Object,
            NullLogger<CreateAgendamentoCommandHandler>.Instance);

        var result = await handler.HandleAsync(new CreateAgendamentoCommand(cliente.Id, slot.Id, veiculo.Id, -1, null));

        Assert.False(result.IsSuccess);
        Assert.Equal("A quilometragem não pode ser negativa.", result.Error);
        agendamentoRepository.Verify(x => x.AddAsync(It.IsAny<Agendamento>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    private static Pessoa CriarPessoaCliente()
        => new("Cliente Teste", new Cpf("39053344705"), new Telefone("11999999999"), new Endereco("Rua Teste", "100", null, "Centro", "São Paulo", "SP", new Cep("01001000")), Guid.NewGuid());

    private static Pessoa CriarPessoaConsultor()
        => new("Consultor Teste", new Cpf("39053344705"), new Telefone("11999999999"), new Endereco("Rua Teste", "100", null, "Centro", "São Paulo", "SP", new Cep("01001000")), Guid.NewGuid());

    private static Veiculo CriarVeiculo(Guid pessoaId)
        => new(pessoaId, new Placa("ABC1234"), "Volkswagen", "Gol", 2020, "Prata", new Hodometro(100000));

    private static AgendaConsultor CriarSlot(Pessoa consultor)
    {
        var slot = new AgendaConsultor(Guid.NewGuid(), Guid.NewGuid(), consultor.Id);
        DefinirPropriedade(slot, nameof(AgendaConsultor.Consultor), consultor);
        return slot;
    }

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
    }
}