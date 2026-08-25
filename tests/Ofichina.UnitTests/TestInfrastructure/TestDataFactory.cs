using Ofichina.Domain.Entities;
using Ofichina.Domain.Aggregates;
using Ofichina.UnitTests.TestInfrastructure.Builders;
using Ofichina.UnitTests.TestInfrastructure.Fakers;

namespace Ofichina.UnitTests.TestInfrastructure;

public static class TestDataFactory
{
    public static class Pessoas
    {
        private static readonly PessoaFaker _faker = new();

        public static Pessoa Criar(Action<Pessoa>? customizar = null)
            => _faker.Criar(customizar);
    }

    public static class Agendamentos
    {
        private static readonly AgendamentoFaker _faker = new();

        public static Agendamento Criar(Guid? clientePessoaId = null, Guid? agendaConsultorId = null, Guid? veiculoId = null, Action<Agendamento>? customizar = null)
            => _faker.Criar(clientePessoaId, agendaConsultorId, veiculoId, customizar);

        public static AgendamentoBuilder Builder()
            => new AgendamentoBuilder();
    }

    public static class AgendaConsultores
    {
        private static readonly AgendaConsultorFaker _faker = new();

        public static AgendaConsultor Criar(Guid? diaDisponibilidadeId = null, Guid? horarioDisponibilidadeId = null, Guid? consultorPessoaId = null, Action<AgendaConsultor>? customizar = null)
            => _faker.Criar(diaDisponibilidadeId, horarioDisponibilidadeId, consultorPessoaId, customizar);

        public static AgendaConsultorBuilder Builder()
            => new AgendaConsultorBuilder();
    }

    public static class Checklists
    {
        private static readonly ChecklistFaker _faker = new();

        public static Checklist Criar(Guid? agendamentoId = null, Action<Checklist>? customizar = null)
            => _faker.Criar(agendamentoId, customizar);

        public static ChecklistBuilder Builder()
            => new ChecklistBuilder();
    }

    public static class DiasDisponibilidade
    {
        private static readonly DiaDisponibilidadeFaker _faker = new();

        public static DiaDisponibilidade Criar(DateOnly? data = null, Action<DiaDisponibilidade>? customizar = null)
            => _faker.Criar(data, customizar);

        public static DiaDisponibilidadeBuilder Builder()
            => new DiaDisponibilidadeBuilder();
    }

    public static class HorariosDisponibilidade
    {
        private static readonly HorarioDisponibilidadeFaker _faker = new();

        public static HorarioDisponibilidade Criar(TimeOnly? hora = null, Action<HorarioDisponibilidade>? customizar = null)
            => _faker.Criar(hora, customizar);

        public static HorarioDisponibilidadeBuilder Builder()
            => new HorarioDisponibilidadeBuilder();
    }

    public static class HorariosConsultor
    {
        private static readonly HorarioConsultorFaker _faker = new();

        public static HorarioConsultor Criar(Guid? horarioDisponibilidadeId = null, Guid? pessoaId = null, Action<HorarioConsultor>? customizar = null)
            => _faker.Criar(horarioDisponibilidadeId, pessoaId, customizar);
    }

    public static class HistoricosStatus
    {
        private static readonly HistoricoStatusFaker _faker = new();

        public static HistoricoStatus ParaOrcamento(Guid? orcamentoId = null, Guid? alteradoPor = null, string? statusAnterior = null, string? statusNovo = null)
            => _faker.ParaOrcamento(orcamentoId, alteradoPor, statusAnterior, statusNovo);

        public static HistoricoStatus ParaOrdemServico(Guid? ordemServicoId = null, Guid? alteradoPor = null, string? statusAnterior = null, string? statusNovo = null)
            => _faker.ParaOrdemServico(ordemServicoId, alteradoPor, statusAnterior, statusNovo);
    }

    public static class MotivosRecusa
    {
        private static readonly MotivoRecusaOrcamentoFaker _faker = new();

        public static MotivoRecusaOrcamento Criar(Guid? orcamentoId = null, Action<MotivoRecusaOrcamento>? customizar = null)
            => _faker.Criar(orcamentoId, customizar);
    }

    public static class Perfis
    {
        private static readonly PerfilFaker _faker = new();

        public static Perfil Criar(Action<Perfil>? customizar = null)
            => _faker.Criar(customizar);
    }

    public static class Permissoes
    {
        private static readonly PermissaoFaker _faker = new();

        public static Permissao Criar(Action<Permissao>? customizar = null)
            => _faker.Criar(customizar);
    }

    public static class Usuarios
    {
        private static readonly UsuarioFaker _faker = new();

        public static Usuario Criar(Action<Usuario>? customizar = null)
            => _faker.Criar(customizar);

        public static UsuarioBuilder Builder()
            => new UsuarioBuilder();
    }

    public static class UsuariosPerfis
    {
        private static readonly UsuarioPerfilFaker _faker = new();

        public static UsuarioPerfil Criar(Guid? usuarioId = null, Guid? perfilId = null, Action<UsuarioPerfil>? customizar = null)
            => _faker.Criar(usuarioId, perfilId, customizar);
    }

    public static class Veiculos
    {
        private static readonly VeiculoFaker _faker = new();

        public static Veiculo Criar(Guid? pessoaId = null, Action<Veiculo>? customizar = null)
            => _faker.Criar(pessoaId, customizar);

        public static VeiculoBuilder Builder()
            => new VeiculoBuilder();
    }

    public static class Servicos
    {
        private static readonly ServicoFaker _faker = new();

        public static Servico Criar(Action<Servico>? customizar = null)
            => _faker.Criar(customizar);
    }

    public static class Pecas
    {
        private static readonly PecaFaker _faker = new();

        public static Peca Criar(Action<Peca>? customizar = null)
            => _faker.Criar(customizar);

        public static PecaBuilder Builder()
            => new PecaBuilder();
    }

    public static class ItensServico
    {
        public static ItemServico ParaOrcamento(Guid orcamentoId)
            => ItemServicoFaker.ParaOrcamento(orcamentoId);

        public static ItemServico ParaOrdemServico(Guid ordemServicoId)
            => ItemServicoFaker.ParaOrdemServico(ordemServicoId);
    }

    public static class Orcamentos
    {
        public static OrcamentoBuilder Builder()
            => new OrcamentoBuilder();
    }

    public static class OrdensServico
    {
        public static OrdemServicoBuilder Builder()
            => new OrdemServicoBuilder();
    }
}
