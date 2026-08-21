using Moq;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Services;
using Ofichina.Contracts.Requests.OrdensServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Application.UseCases.OrdensServico;

public sealed class CreateOrdemServicoServiceTests
{
    // ============================================================  
    // Pessoa  
    // ============================================================  

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Pessoa_Nao_Existir()
    {
        var command = CriarCommand();

        var pessoaRepository = new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Pessoa?)null);

        var service = CriarService(pessoaRepository: pessoaRepository);

        var result = await service.CreateAsync(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Pessoa não encontrada.", result.Error);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Pessoa_Estiver_Excluida()
    {
        var command = CriarCommand();

        var pessoaRepository = new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPessoaExcluida());

        var service = CriarService(pessoaRepository: pessoaRepository);

        var result = await service.CreateAsync(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Pessoa não encontrada.", result.Error);
    }

    // ============================================================  
    // Funcionário (Consultor)  
    // ============================================================  

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Funcionario_Nao_Existir()
    {
        var command = CriarCommand();

        var pessoaRepository = new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPessoa());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Pessoa?)null);

        var service = CriarService(pessoaRepository: pessoaRepository);

        var result = await service.CreateAsync(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Funcionário não encontrado.", result.Error);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Funcionario_Estiver_Excluido()
    {
        var command = CriarCommand();

        var pessoaRepository = new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPessoa());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPessoaExcluida());

        var service = CriarService(pessoaRepository: pessoaRepository);

        var result = await service.CreateAsync(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Funcionário não encontrado.", result.Error);
    }

    // ============================================================  
    // Veículo  
    // ============================================================  

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Veiculo_Nao_Existir()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepositoryValido(command);

        var veiculoRepository = new Mock<IRepository<Veiculo>>();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Veiculo?)null);

        var service = CriarService(
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository);

        var result = await service.CreateAsync(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Veículo não encontrado.", result.Error);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Veiculo_Estiver_Excluido()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepositoryValido(command);

        var veiculoRepository = new Mock<IRepository<Veiculo>>();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarVeiculoExcluido());

        var service = CriarService(
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository);

        var result = await service.CreateAsync(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Veículo não encontrado.", result.Error);
    }

    // ============================================================  
    // Sucesso  
    // ============================================================  

    [Fact]
    public async Task CreateAsync_Deve_Criar_Ordem_Servico_Com_Sucesso()
    {
        var command = CriarCommand();

        var pessoaRepository = CriarPessoaRepositoryValido(command);

        var veiculoRepository = new Mock<IRepository<Veiculo>>();

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarVeiculo());

        var ordemServicoRepository = new Mock<IRepository<OrdemServico>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var service = CriarService(
            ordemServicoRepository: ordemServicoRepository,
            pessoaRepository: pessoaRepository,
            veiculoRepository: veiculoRepository,
            unitOfWork: unitOfWork);

        var result = await service.CreateAsync(command, CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error);

        ordemServicoRepository.Verify(
            x => x.AddAsync(
                It.Is<OrdemServico>(o =>
                    o.PessoaId == command.PessoaId &&
                    o.VeiculoId == command.VeiculoId &&
                    o.ConsultorId == command.ConsultorId &&
                    o.Hodometro == command.Hodometro &&
                    o.ProblemaRelatado == command.ProblemaRelatado),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================  
    // DomainException  
    // ============================================================  

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Ocorrer_DomainException()
    {
        var command = CriarCommand();

        var pessoaRepository = new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ThrowsAsync(new DomainException("Erro de domínio."));

        var service = CriarService(pessoaRepository: pessoaRepository);

        var result = await service.CreateAsync(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Erro de domínio.", result.Error);
    }

    // ============================================================  
    // Exception genérica  
    // ============================================================  

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Falha_Quando_Ocorrer_Excecao()
    {
        var command = CriarCommand();

        var pessoaRepository = new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ThrowsAsync(new InvalidOperationException("Erro inesperado."));

        var service = CriarService(pessoaRepository: pessoaRepository);

        var result = await service.CreateAsync(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível criar a ordem de serviço.", result.Error);
    }

    // ============================================================  
    // Helpers  
    // ============================================================  

    private static CreateOrdemServicoService CriarService(
        Mock<IRepository<OrdemServico>>? ordemServicoRepository = null,
        Mock<IRepository<Pessoa>>? pessoaRepository = null,
        Mock<IRepository<Veiculo>>? veiculoRepository = null,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        return new CreateOrdemServicoService(
            (ordemServicoRepository ?? new Mock<IRepository<OrdemServico>>()).Object,
            (pessoaRepository ?? new Mock<IRepository<Pessoa>>()).Object,
            (veiculoRepository ?? new Mock<IRepository<Veiculo>>()).Object,
            (unitOfWork ?? new Mock<IUnitOfWork>()).Object);
    }

    private static Mock<IRepository<Pessoa>> CriarPessoaRepositoryValido(
        CreateOrdemServicoCommand command)
    {
        var pessoaRepository = new Mock<IRepository<Pessoa>>();

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPessoa());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPessoa());

        return pessoaRepository;
    }

    private static CreateOrdemServicoCommand CriarCommand()
    {
        return new CreateOrdemServicoCommand(
            new CreateOrdemServicoRequest
            {
                PessoaId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ConsultorId = Guid.NewGuid(),
                Hodometro = 35_000,
                ProblemaRelatado = "Ruído no motor.",
                Observacoes = "Avaliar correia."
            });
    }

    private static Pessoa CriarPessoa()
    {
        return new Pessoa(
            "João da Silva",
            new Cpf("12345678909"),
            new Telefone("11999999999"),
            new Endereco(
                "Rua Exemplo",
                "123",
                "",
                "Bairro Exemplo",
                "Cidade Exemplo",
                "Estado Exemplo",
                new Cep("12345-678")),
            Guid.NewGuid());
    }

    private static Pessoa CriarPessoaExcluida()
    {
        var pessoa = CriarPessoa();
        pessoa.DeletedAt = DateTime.UtcNow;
        return pessoa;
    }

    private static Veiculo CriarVeiculo()
    {
        return new Veiculo(
            Guid.NewGuid(),
            new Placa("ABC1D23"),
            "Ford",
            "Ka",
            2022,
            "Prata",
            new Hodometro(10000));
    }

    private static Veiculo CriarVeiculoExcluido()
    {
        var veiculo = CriarVeiculo();
        veiculo.DeletedAt = DateTime.UtcNow;
        return veiculo;
    }
}