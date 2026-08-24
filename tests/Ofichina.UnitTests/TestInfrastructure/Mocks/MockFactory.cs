using Moq;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure;

public static class MockFactory
{
    public static class Repositorio<T> where T : Entity
    {
        public static Mock<IRepository<T>> ComGetById(T? entidade)
        {
            var mock = new Mock<IRepository<T>>();
            if (entidade != null)
            {
                mock.Setup(m => m.GetByIdAsync(entidade.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                    .ReturnsAsync(entidade);
            }

            return mock;
        }
    }

    public static class PessoaRepository
    {
        public static Mock<IPessoaRepository> ComGetById(Pessoa? pessoa, bool includeVeiculos = false)
        {
            var mock = new Mock<IPessoaRepository>();
            if (pessoa != null)
            {
                mock.Setup(m => m.GetByIdAsync(pessoa.Id, includeVeiculos, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(pessoa);
            }

            return mock;
        }

        public static Mock<IPessoaRepository> ComGetByUsuarioId(Pessoa? pessoa)
        {
            var mock = new Mock<IPessoaRepository>();
            if (pessoa != null)
            {
                mock.Setup(m => m.GetByUsuarioIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(pessoa);
            }

            return mock;
        }
    }

    public static class PecaRepository
    {
        public static Mock<IPecaRepository> ComGetById(Peca? peca)
        {
            var mock = new Mock<IPecaRepository>();
            if (peca != null)
            {
                mock.Setup(m => m.GetByIdAsync(peca.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                    .ReturnsAsync(peca);
            }

            return mock;
        }
    }

    public static class ServicoRepository
    {
        public static Mock<IServicoRepository> ComGetById(Servico? servico)
        {
            var mock = new Mock<IServicoRepository>();
            if (servico != null)
            {
                mock.Setup(m => m.GetByIdAsync(servico.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                    .ReturnsAsync(servico);
            }

            return mock;
        }
    }

    public static class VeiculoRepository
    {
        public static Mock<IVeiculoRepository> ComGetById(Veiculo? veiculo, bool includePessoa = false)
        {
            var mock = new Mock<IVeiculoRepository>();
            if (veiculo != null)
            {
                mock.Setup(m => m.GetByIdAsync(veiculo.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(veiculo);
            }

            return mock;
        }

        public static Mock<IVeiculoRepository> ComGetByIdAndGetAll(Veiculo? veiculo, IEnumerable<Veiculo>? all = null, bool includePessoa = false)
        {
            var mock = new Mock<IVeiculoRepository>();
            if (veiculo != null)
            {
                mock.Setup(m => m.GetByIdAsync(veiculo.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(veiculo);
            }

            if (all != null)
            {
                mock.Setup(m => m.GetAllAsync(includePessoa: includePessoa, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(all);
            }

            return mock;
        }
    }

    public static class OrcamentoRepository
    {
        public static Mock<IOrcamentoRepository> ComGetById(Orcamento? orcamento, bool includeItens = false)
        {
            var mock = new Mock<IOrcamentoRepository>();
            if (orcamento != null)
            {
                mock.Setup(m => m.GetByIdAsync(orcamento.Id, includeItens, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                    .ReturnsAsync(orcamento);
            }

            return mock;
        }
    }

    public static class OrdemServicoRepository
    {
        public static Mock<IOrdemServicoRepository> ComGetById(OrdemServico? ordemServico, bool includeItens = false)
        {
            var mock = new Mock<IOrdemServicoRepository>();
            if (ordemServico != null)
            {
                mock.Setup(m => m.GetByIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(ordemServico);
            }

            return mock;
        }
    }

    public static class UnitOfWork
    {
        public static Mock<IUnitOfWork> Default()
        {
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(m => m.SaveChangesAsync()).ReturnsAsync(1);
            return mock;
        }
    }
}
