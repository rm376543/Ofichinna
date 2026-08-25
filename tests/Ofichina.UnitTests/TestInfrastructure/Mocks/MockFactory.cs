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

    public static class AgendamentoRepository
    {
        public static Mock<IAgendamentoRepository> ComGetById(Agendamento? agendamento)
        {
            var mock = new Mock<IAgendamentoRepository>();
            if (agendamento != null)
            {
                mock.Setup(m => m.GetByIdAsync(agendamento.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                    .ReturnsAsync(agendamento);
            }

            return mock;
        }

        public static Mock<IAgendamentoRepository> ComGetAllWithIncludes(IEnumerable<Agendamento>? all = null)
        {
            var mock = new Mock<IAgendamentoRepository>();
            if (all != null)
            {
                mock.Setup(m => m.GetAllWithIncludesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(all.ToList().AsReadOnly());
            }

            return mock;
        }

        public static Mock<IAgendamentoRepository> ComGetByIdAndPessoa(Agendamento? agendamento, Guid? pessoaId = null)
        {
            var mock = new Mock<IAgendamentoRepository>();
            if (agendamento != null)
            {
                mock.Setup(m => m.GetByIdAndPessoaAsync(agendamento.Id, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(agendamento);
            }

            return mock;
        }

        public static Mock<IAgendamentoRepository> ComExisteConflitoConsultor(bool existe)
        {
            var mock = new Mock<IAgendamentoRepository>();
            mock.Setup(m => m.ExisteConflitoConsultorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existe);
            return mock;
        }

        public static Mock<IAgendamentoRepository> ComExisteConflitoVeiculo(bool existe)
        {
            var mock = new Mock<IAgendamentoRepository>();
            mock.Setup(m => m.ExisteConflitoVeiculoAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existe);
            return mock;
        }
    }

    public static class ChecklistRepository
    {
        public static Mock<IChecklistRepository> ComGetByAgendamentoChecklistId(Checklist? checklist)
        {
            var mock = new Mock<IChecklistRepository>();
            if (checklist != null)
            {
                mock.Setup(m => m.GetByAgendamentoChecklistIdAsync(It.IsAny<Guid>(), checklist.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                    .ReturnsAsync(checklist);
            }

            return mock;
        }
    }

    public static class HorarioConsultorRepository
    {
        public static Mock<IHorarioConsultorRepository> ComGetConsultoresPorHorario(IEnumerable<HorarioConsultor>? consultores = null)
        {
            var mock = new Mock<IHorarioConsultorRepository>();
            if (consultores != null)
            {
                mock.Setup(m => m.GetConsultoresPorHorarioAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(consultores.ToList().AsReadOnly());
            }

            return mock;
        }
    }

    public static class HorarioDisponibilidadeRepository
    {
        public static Mock<IHorarioDisponibilidadeRepository> ComGetHorariosPorDia(IEnumerable<HorarioDisponibilidade>? horarios = null)
        {
            var mock = new Mock<IHorarioDisponibilidadeRepository>();
            if (horarios != null)
            {
                mock.Setup(m => m.GetHorariosPorDiaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(horarios.ToList().AsReadOnly());
            }

            return mock;
        }

        public static Mock<IHorarioDisponibilidadeRepository> ComBuscarPorHorario(HorarioDisponibilidade? horario)
        {
            var mock = new Mock<IHorarioDisponibilidadeRepository>();
            if (horario != null)
            {
                mock.Setup(m => m.BuscarPorHorarioAsync(It.IsAny<TimeOnly>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(horario);
            }

            return mock;
        }
    }

    public static class DiaDisponibilidadeRepository
    {
        public static Mock<IDiaDisponibilidadeRepository> ComGetDiasDisponiveis(IEnumerable<DiaDisponibilidade>? dias = null)
        {
            var mock = new Mock<IDiaDisponibilidadeRepository>();
            if (dias != null)
            {
                mock.Setup(m => m.GetDiasDisponiveisAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(dias.ToList().AsReadOnly());
            }

            return mock;
        }
    }

    public static class PerfilRepository
    {
        public static Mock<IPerfilRepository> ComGetById(Perfil? perfil)
        {
            var mock = new Mock<IPerfilRepository>();
            if (perfil != null)
            {
                mock.Setup(m => m.GetByIdAsync(perfil.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                    .ReturnsAsync(perfil);
            }

            return mock;
        }

        public static Mock<IPerfilRepository> ComGetByNome(Perfil? perfil)
        {
            var mock = new Mock<IPerfilRepository>();
            if (perfil != null)
            {
                mock.Setup(m => m.GetByNomeAsync(perfil.NomePerfil, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(perfil);
            }

            return mock;
        }

        public static Mock<IPerfilRepository> ComGetAllAtivos(IEnumerable<Perfil>? perfis = null)
        {
            var mock = new Mock<IPerfilRepository>();
            if (perfis != null)
            {
                mock.Setup(m => m.GetAllAtivosAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(perfis);
            }

            return mock;
        }
    }

    public static class PermissaoRepository
    {
        public static Mock<IPermissaoRepository> ComGetByCodigo(Permissao? permissao)
        {
            var mock = new Mock<IPermissaoRepository>();
            if (permissao != null)
            {
                mock.Setup(m => m.GetByCodigoAsync(permissao.Codigo, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(permissao);
            }

            return mock;
        }
    }

    public static class PerfilPermissaoRepository
    {
        public static Mock<IPerfilPermissaoRepository> ComGetByPerfilIdPermissaoId(PerfilPermissao? perfilPermissao)
        {
            var mock = new Mock<IPerfilPermissaoRepository>();
            if (perfilPermissao != null)
            {
                mock.Setup(m => m.GetByPerfilIdPermissaoIdAsync(perfilPermissao.PerfilId, perfilPermissao.PermissaoId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(perfilPermissao);
            }

            return mock;
        }

        public static Mock<IPerfilPermissaoRepository> ComGetByPerfilId(IEnumerable<PerfilPermissao>? perfis = null)
        {
            var mock = new Mock<IPerfilPermissaoRepository>();
            if (perfis != null)
            {
                mock.Setup(m => m.GetByPerfilIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(perfis.ToList().AsReadOnly());
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
