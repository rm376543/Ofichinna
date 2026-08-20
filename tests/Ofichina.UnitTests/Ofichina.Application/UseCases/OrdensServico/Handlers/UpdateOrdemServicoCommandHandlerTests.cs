using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Handlers;
using Ofichina.Contracts.Requests.OrdensServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Application.UseCases.OrdensServico.Handlers;

public sealed class UpdateOrdemServicoCommandHandlerTests
{
    // ============================================================
    // HandleAsync - Sucesso
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Atualizar_Ordem_De_Servico_Com_Sucesso()
    {
        var ordemServicoRepository =
            new Mock<IRepository<OrdemServico>>();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        var veiculoRepository =
            new Mock<IRepository<Veiculo>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var ordemServico = CriarOrdemServico();
        var pessoa = CriarPessoa();
        var funcionario = CriarPessoa();
        var veiculo = CriarVeiculo(pessoa.Id);

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(ordemServico);

        pessoaRepository
            .SetupSequence(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa)
            .ReturnsAsync(funcionario);

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(veiculo);

        var handler = CriarHandler(
            ordemServicoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        Assert.Equal(command.PessoaId, ordemServico.PessoaId);
        Assert.Equal(command.VeiculoId, ordemServico.VeiculoId);
        Assert.Equal(command.ConsultorId, ordemServico.ConsultorId);
        Assert.Equal(command.Hodometro, ordemServico.Hodometro);
        Assert.Equal(
            command.ProblemaRelatado,
            ordemServico.ProblemaRelatado);
        Assert.Equal(
            command.Observacoes,
            ordemServico.Observacao);

        ordemServicoRepository.Verify(
            x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        veiculoRepository.Verify(
            x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        ordemServicoRepository.Verify(
            x => x.UpdateAsync(
                ordemServico,
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    // ============================================================
    // HandleAsync - Ordem de serviço não encontrada
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ordem_De_Servico_Nao_Existir()
    {
        var ordemServicoRepository =
            new Mock<IRepository<OrdemServico>>();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        var veiculoRepository =
            new Mock<IRepository<Veiculo>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((OrdemServico?)null);

        var handler = CriarHandler(
            ordemServicoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Ordem de serviço não encontrada.",
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

        ordemServicoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Ordem de serviço excluída
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ordem_De_Servico_Estiver_Excluida()
    {
        var ordemServicoRepository =
            new Mock<IRepository<OrdemServico>>();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        var veiculoRepository =
            new Mock<IRepository<Veiculo>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var ordemServico = CriarOrdemServico();
        ordemServico.Excluir();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(ordemServico);

        var handler = CriarHandler(
            ordemServicoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Ordem de serviço não encontrada.",
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

        ordemServicoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Pessoa não encontrada
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Pessoa_Nao_Existir()
    {
        var ordemServicoRepository =
            new Mock<IRepository<OrdemServico>>();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        var veiculoRepository =
            new Mock<IRepository<Veiculo>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarOrdemServico());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Pessoa?)null);

        var handler = CriarHandler(
            ordemServicoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Pessoa não encontrada.",
            result.Error);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        veiculoRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Pessoa excluída
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Pessoa_Estiver_Excluida()
    {
        var ordemServicoRepository =
            new Mock<IRepository<OrdemServico>>();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        var veiculoRepository =
            new Mock<IRepository<Veiculo>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var pessoa = CriarPessoa();
        pessoa.Excluir();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarOrdemServico());

        pessoaRepository
            .Setup(x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(pessoa);

        var handler = CriarHandler(
            ordemServicoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Pessoa não encontrada.",
            result.Error);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        veiculoRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Funcionário não encontrado
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Funcionario_Nao_Existir()
    {
        var ordemServicoRepository =
            new Mock<IRepository<OrdemServico>>();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        var veiculoRepository =
            new Mock<IRepository<Veiculo>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarOrdemServico());

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

        var handler = CriarHandler(
            ordemServicoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Funcionário não encontrado.",
            result.Error);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                command.PessoaId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        pessoaRepository.Verify(
            x => x.GetByIdAsync(
                command.ConsultorId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        veiculoRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Funcionário excluído
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Funcionario_Estiver_Excluido()
    {
        var ordemServicoRepository =
            new Mock<IRepository<OrdemServico>>();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        var veiculoRepository =
            new Mock<IRepository<Veiculo>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var funcionario = CriarPessoa();
        funcionario.Excluir();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarOrdemServico());

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
            .ReturnsAsync(funcionario);

        var handler = CriarHandler(
            ordemServicoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Funcionário não encontrado.",
            result.Error);

        veiculoRepository.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Never);

        ordemServicoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Veículo não encontrado
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Veiculo_Nao_Existir()
    {
        var ordemServicoRepository =
            new Mock<IRepository<OrdemServico>>();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        var veiculoRepository =
            new Mock<IRepository<Veiculo>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarOrdemServico());

        pessoaRepository
            .SetupSequence(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPessoa())
            .ReturnsAsync(CriarPessoa());

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Veiculo?)null);

        var handler = CriarHandler(
            ordemServicoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Veículo não encontrado.",
            result.Error);

        veiculoRepository.Verify(
            x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

        ordemServicoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Veículo excluído
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Veiculo_Estiver_Excluido()
    {
        var ordemServicoRepository =
            new Mock<IRepository<OrdemServico>>();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        var veiculoRepository =
            new Mock<IRepository<Veiculo>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var veiculo = CriarVeiculo(command.PessoaId);
        veiculo.Desativar();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarOrdemServico());

        pessoaRepository
            .SetupSequence(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPessoa())
            .ReturnsAsync(CriarPessoa());

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(veiculo);

        var handler = CriarHandler(
            ordemServicoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Veículo não encontrado.",
            result.Error);

        ordemServicoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - DomainException
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrrer_Excecao_De_Dominio()
    {
        var ordemServicoRepository =
            new Mock<IRepository<OrdemServico>>();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        var veiculoRepository =
            new Mock<IRepository<Veiculo>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        var ordemServico = CriarOrdemServico();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(ordemServico);

        pessoaRepository
            .SetupSequence(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarPessoa())
            .ReturnsAsync(CriarPessoa());

        veiculoRepository
            .Setup(x => x.GetByIdAsync(
                command.VeiculoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ReturnsAsync(CriarVeiculo(command.PessoaId));

        // Força a exceção no UpdateAsync para exercitar o catch
        // específico de DomainException.
        ordemServicoRepository
            .Setup(x => x.UpdateAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new DomainException(
                    "Erro de domínio ao atualizar a ordem."));

        var handler = CriarHandler(
            ordemServicoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Erro de domínio ao atualizar a ordem.",
            result.Error);

        ordemServicoRepository.Verify(
            x => x.UpdateAsync(
                ordemServico,
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // HandleAsync - Exception
    // ============================================================

    [Fact]
    public async Task HandleAsync_Deve_Retornar_Falha_Quando_Ocorrrer_Excecao()
    {
        var ordemServicoRepository =
            new Mock<IRepository<OrdemServico>>();

        var pessoaRepository =
            new Mock<IRepository<Pessoa>>();

        var veiculoRepository =
            new Mock<IRepository<Veiculo>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        var command = CriarCommand();

        ordemServicoRepository
            .Setup(x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro inesperado."));

        var handler = CriarHandler(
            ordemServicoRepository,
            pessoaRepository,
            veiculoRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "Não foi possível atualizar a ordem de serviço.",
            result.Error);

        ordemServicoRepository.Verify(
            x => x.GetByIdAsync(
                command.OrdemServicoId,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()),
            Times.Once);

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

        ordemServicoRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    // ============================================================
    // Helpers
    // ============================================================

    private static UpdateOrdemServicoCommand CriarCommand()
    {
        var request = new UpdateOrdemServicoRequest
        {
            OrdemServicoId = Guid.NewGuid(),
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ConsultorId = Guid.NewGuid(),
            Hodometro = 50_000,
            ProblemaRelatado = "Motor apresentando ruído.",
            Observacoes = "Atualização realizada durante atendimento."
        };

        return new UpdateOrdemServicoCommand(request);
    }

    private static OrdemServico CriarOrdemServico()
    {
        return new OrdemServico(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10_000,
            "Veículo apresentando falha.",
            "Observação inicial.");
    }

    private static Pessoa CriarPessoa()
    {
        return new Pessoa(
            "João da Silva",
            new Cpf("12345678909"),
            new Telefone("16999999999"),
            new Endereco(
                "Rua das Flores",
                "123",
                "",
                "Centro",
                "São José do Rio Preto",
                "SP",
                new Cep("15000000")),
            Guid.NewGuid());
    }

    private static Veiculo CriarVeiculo(Guid pessoaId)
    {
        return new Veiculo(
            pessoaId,
            new Placa("ABC1D23"),
            "Toyota",
            "Corolla",
            2022,
            "Prata",
            new Hodometro(50_000));
    }

    private static UpdateOrdemServicoCommandHandler CriarHandler(
        Mock<IRepository<OrdemServico>> ordemServicoRepository,
        Mock<IRepository<Pessoa>> pessoaRepository,
        Mock<IRepository<Veiculo>> veiculoRepository,
        Mock<IUnitOfWork> unitOfWork)
        => new(
            ordemServicoRepository.Object,
            pessoaRepository.Object,
            veiculoRepository.Object,
            unitOfWork.Object,
            NullLogger<UpdateOrdemServicoCommandHandler>.Instance);
}