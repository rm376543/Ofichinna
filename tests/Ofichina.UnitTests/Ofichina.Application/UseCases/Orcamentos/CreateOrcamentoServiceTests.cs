using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Application.UseCases.Orcamentos.Services;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.UnitTests.TestInfrastructure;

namespace Ofichina.UnitTests.Application.UseCases.Orcamentos;

public sealed class CreateOrcamentoServiceTests
{
    // ============================================================
    // Pessoa
    // ============================================================

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Pessoa_Nao_Existir()
    {
        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pessoa?)null);

        var service = CriarService(
            pessoaRepository: pessoaRepository);

        var result = await service.CreateAsync(
            CriarCommand(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Pessoa não encontrada.",
            result.Error);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Pessoa_Estiver_Excluida()
    {
        var pessoa = CriarPessoaExcluida();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        var service = CriarService(
            pessoaRepository: pessoaRepository);

        var result = await service.CreateAsync(
            CriarCommand(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Pessoa não encontrada.",
            result.Error);
    }

    // ============================================================
    // Veículo
    // ============================================================

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Veiculo_Nao_Existir()
    {
        var pessoa = CriarPessoa();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        var veiculoRepository = CriarVeiculoRepository();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Veiculo?)null);

        var service = CriarService(
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository);

        var result = await service.CreateAsync(
            CriarCommand(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Veículo não encontrado.",
            result.Error);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Veiculo_Estiver_Excluido()
    {
        var pessoa = CriarPessoa();
        var veiculo = CriarVeiculoExcluido();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        var veiculoRepository = CriarVeiculoRepository();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        var service = CriarService(
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository);

        var result = await service.CreateAsync(
            CriarCommand(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Veículo não encontrado.",
            result.Error);
    }

    // ============================================================
    // Agendamento
    // ============================================================

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Agendamento_Nao_Existir()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        var veiculoRepository = CriarVeiculoRepository();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarVeiculo());

        var agendamentoRepository =
            new Mock<IAgendamentoRepository>();

        agendamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.AgendamentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Agendamento?)null);

        var service = CriarService(
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository,
            agendamentoRepository: agendamentoRepository);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Agendamento não encontrado.",
            result.Error);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Agendamento_Estiver_Excluido()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        var veiculoRepository = CriarVeiculoRepository();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarVeiculo());

        var agendamentoRepository =
            new Mock<IAgendamentoRepository>();

        agendamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.AgendamentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                CriarAgendamentoExcluido(
                    command.PessoaId,
                    command.AgendamentoId,
                    command.VeiculoId));

        var service = CriarService(
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository,
            agendamentoRepository: agendamentoRepository);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Agendamento não encontrado.",
            result.Error);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Agendamento_Nao_Corresponder()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        var veiculoRepository = CriarVeiculoRepository();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarVeiculo());

        var agendamentoRepository =
            new Mock<IAgendamentoRepository>();

        agendamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.AgendamentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                CriarAgendamento(
                    Guid.NewGuid(),
                    command.AgendamentoId,
                    command.VeiculoId));

        var service = CriarService(
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository,
            agendamentoRepository: agendamentoRepository);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "O agendamento informado não corresponde à pessoa e ao veículo do orçamento.",
            result.Error);
    }

    // ============================================================
    // Checklist
    // ============================================================

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Nao_Houver_Checklist()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        var veiculoRepository = CriarVeiculoRepository();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarVeiculo());

        var agendamentoRepository =
            new Mock<IAgendamentoRepository>();

        agendamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.AgendamentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                CriarAgendamento(
                    command.PessoaId,
                    command.AgendamentoId,
                    command.VeiculoId));

        var checklistRepository =
            new Mock<IRepository<Checklist>>();

        checklistRepository
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var service = CriarService(
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository,
            agendamentoRepository: agendamentoRepository,
            checklistRepository: checklistRepository);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Nenhum checklist encontrado para o agendamento informado.",
            result.Error);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Houver_Checklist_Pendente()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        var veiculoRepository = CriarVeiculoRepository();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarVeiculo());

        var agendamentoRepository =
            new Mock<IAgendamentoRepository>();

        agendamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.AgendamentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                CriarAgendamento(
                    command.PessoaId,
                    command.AgendamentoId,
                    command.VeiculoId));

        var checklistRepository =
            new Mock<IRepository<Checklist>>();

        checklistRepository
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                CriarChecklist(
                    command.AgendamentoId,
                    false)
            ]);

        var service = CriarService(
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository,
            agendamentoRepository: agendamentoRepository,
            checklistRepository: checklistRepository);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Existem checklists pendentes para o agendamento informado.",
            result.Error);
    }

    // ============================================================
    // Mecânico
    // ============================================================

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Mecanico_Nao_Existir()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.MecanicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pessoa?)null);

        var service = CriarServiceComChecklistValido(
            command,
            pessoaRepository);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Mecânico do diagnóstico não encontrado.",
            result.Error);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Mecanico_Estiver_Excluido()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.MecanicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoaExcluida());

        var service = CriarServiceComChecklistValido(
            command,
            pessoaRepository);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Mecânico do diagnóstico não encontrado.",
            result.Error);
    }

    // ============================================================
    // Consultor
    // ============================================================

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Consultor_Nao_Existir()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.MecanicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pessoa?)null);

        var service = CriarServiceComChecklistValido(
            command,
            pessoaRepository);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Consultor não encontrado.",
            result.Error);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Consultor_Estiver_Excluido()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.MecanicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoaExcluida());

        var service = CriarServiceComChecklistValido(
            command,
            pessoaRepository);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Consultor não encontrado.",
            result.Error);
    }

    // ============================================================
    // Sucesso
    // ============================================================

    [Fact]
    public async Task CreateAsync_Deve_Criar_Orcamento_Com_Sucesso()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.MecanicoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPessoa());

        var veiculoRepository = CriarVeiculoRepository();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarVeiculo());

        var agendamentoRepository =
            new Mock<IAgendamentoRepository>();

        agendamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.AgendamentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                CriarAgendamento(
                    command.PessoaId,
                    command.AgendamentoId,
                    command.VeiculoId));

        var checklistRepository =
            new Mock<IRepository<Checklist>>();

        checklistRepository
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                CriarChecklist(
                    command.AgendamentoId,
                    true)
            ]);

        var orcamentoRepository =
            new Mock<IRepository<Orcamento>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var service = CriarService(
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository,
            agendamentoRepository: agendamentoRepository,
            checklistRepository: checklistRepository,
            orcamentoRepository: orcamentoRepository,
            unitOfWork: unitOfWork);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        orcamentoRepository.Verify(
            x => x.AddAsync(
                It.Is<Orcamento>(o =>
                    o.PessoaId == command.PessoaId &&
                    o.VeiculoId == command.VeiculoId &&
                    o.AgendamentoId == command.AgendamentoId &&
                    o.MecanicoId == command.MecanicoId &&
                    o.ConsultorId == command.ConsultorId)),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // DomainException
    // ============================================================

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Ocorrrer_DomainException()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new DomainException(
                    "Erro de domínio."));

        var service = CriarService(
            pessoaRepository: pessoaRepository);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Erro de domínio.",
            result.Error);
    }

    // ============================================================
    // Exception
    // ============================================================

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Ocorrrer_Excecao()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepository();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var service = CriarService(
            pessoaRepository: pessoaRepository);

        var result = await service.CreateAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Não foi possível criar o orçamento.",
            result.Error);
    }

    // ============================================================
    // Helpers
    // ============================================================

    private static CreateOrcamentoService CriarService(
        Mock<IRepository<Orcamento>>? orcamentoRepository = null,
        Mock<IRepository<Checklist>>? checklistRepository = null,
        Mock<IAgendamentoRepository>? agendamentoRepository = null,
        Mock<IRepository<Pessoa>>? pessoaRepository = null,
        Mock<IRepository<Veiculo>>? veiculoRepository = null,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        return new CreateOrcamentoService(
            (orcamentoRepository ?? new Mock<IRepository<Orcamento>>()).Object,
            (checklistRepository ?? new Mock<IRepository<Checklist>>()).Object,
            (agendamentoRepository ?? new Mock<IAgendamentoRepository>()).Object,
            (pessoaRepository ?? new Mock<IRepository<Pessoa>>()).Object,
            (veiculoRepository ?? new Mock<IRepository<Veiculo>>()).Object,
            (unitOfWork ?? new Mock<IUnitOfWork>()).Object);
    }

    private static Mock<IRepository<Pessoa>> CriarPessoaRepository()
        => new();

    private static Mock<IRepository<Veiculo>> CriarVeiculoRepository()
        => new();

    private static CreateOrcamentoCommand CriarCommand()
    {
        return new CreateOrcamentoCommand(
            new CreateOrcamentoRequest
            {
                PessoaId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                AgendamentoId = Guid.NewGuid(),
                MecanicoId = Guid.NewGuid(),
                ConsultorId = Guid.NewGuid(),
                DataValidade = DateOnly.FromDateTime(
                    DateTime.UtcNow.AddDays(30)),
                Observacoes = "Observações do orçamento."
            });
    }

    private static Pessoa CriarPessoa()
    {
        return TestDataFactory.Pessoas.Criar(p => { });
    }

    private static Pessoa CriarPessoaExcluida()
    {
        var pessoa = CriarPessoa();
        pessoa.DeletedAt = DateTime.UtcNow;
        return pessoa;
    }

    private static Veiculo CriarVeiculo()
    {
        return TestDataFactory.Veiculos.Criar();
    }

    private static Veiculo CriarVeiculoExcluido()
    {
        var veiculo = CriarVeiculo();
        veiculo.DeletedAt = DateTime.UtcNow;
        return veiculo;
    }

    private static Agendamento CriarAgendamento(
        Guid clientePessoaId,
        Guid agendaConsultorId,
        Guid veiculoId)
    {
        return new Agendamento(
            clientePessoaId,
            agendaConsultorId,
            veiculoId);
    }

    private static Agendamento CriarAgendamentoExcluido(
        Guid clientePessoaId,
        Guid agendaConsultorId,
        Guid veiculoId)
    {
        var agendamento = CriarAgendamento(
            clientePessoaId,
            agendaConsultorId,
            veiculoId);

        agendamento.DeletedAt = DateTime.UtcNow;

        return agendamento;
    }

    private static Checklist CriarChecklist(
        Guid agendamentoId,
        bool finalizado)
    {
        var checklist = new Checklist(agendamentoId, "Parte eletrica ok", "");

        if (finalizado)
        {
            checklist.Finalizar();
        }

        return checklist;
    }

    private static CreateOrcamentoService CriarServiceComChecklistValido(
        CreateOrcamentoCommand command,
        Mock<IRepository<Pessoa>> pessoaRepository)
    {
        var veiculoRepository = CriarVeiculoRepository();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarVeiculo());

        var agendamentoRepository =
            new Mock<IAgendamentoRepository>();

        agendamentoRepository
            .Setup(x => x.GetByIdAsync(
                command.AgendamentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                CriarAgendamento(
                    command.PessoaId,
                    command.AgendamentoId,
                    command.VeiculoId));

        var checklistRepository =
            new Mock<IRepository<Checklist>>();

        checklistRepository
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                CriarChecklist(
                    command.AgendamentoId,
                    true)
            ]);

        return CriarService(
            checklistRepository: checklistRepository,
            agendamentoRepository: agendamentoRepository,
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository);
    }
}