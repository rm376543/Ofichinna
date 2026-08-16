using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Ofichina.Infrastructure.Migrations;

namespace Ofichina.UnitTests.Migrations;

public sealed class _001inicialTests
{
    [Fact]
    public void Up_Deve_Criar_Tabelas_Indices_E_Relacionamentos_Esperados()
    {
        // Arrange
        var migration = new MigrationProxy();
        var migrationBuilder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        // Act
        migration.ApplyUp(migrationBuilder);

        // Assert
        var createTables = migrationBuilder.Operations.OfType<CreateTableOperation>().Select(operation => operation.Name).ToArray();
        var createIndexes = migrationBuilder.Operations.OfType<CreateIndexOperation>().Select(operation => operation.Name).ToArray();

        Assert.Equal(63, migrationBuilder.Operations.Count);
        Assert.Equal(new[]
        {
            "DiasDisponibilidade",
            "HorariosDisponibilidade",
            "Pecas",
            "Perfis",
            "Permissoes",
            "Servicos",
            "Usuarios",
            "DiasHorariosDisponibilidade",
            "PerfisPermissoes",
            "Pessoas",
            "UsuariosPerfis",
            "AgendaConsultor",
            "HorariosConsultores",
            "Veiculos",
            "Agendamentos",
            "OrdensServico",
            "Checklists",
            "Orcamentos",
            "HistoricoStatus",
            "ItensServico",
            "MotivosRecusaOrcamento"
        }, createTables);

        Assert.Equal(new[]
        {
            "IX_AgendaConsultor_ConsultorPessoaId",
            "IX_AgendaConsultor_DiaDisponibilidadeId",
            "IX_AgendaConsultor_DiaHorarioConsultor",
            "IX_AgendaConsultor_HorarioDisponibilidadeId",
            "IX_Agendamentos_AgendaConsultorId",
            "IX_Agendamentos_ClientePessoaId",
            "IX_Agendamentos_Status",
            "IX_Agendamentos_VeiculoId",
            "IX_Checklists_AgendamentoId",
            "IX_DiasDisponibilidade_Data",
            "IX_DiasHorariosDisponibilidade_DiaDisponibilidadeId_HorarioDisponibilidadeId",
            "IX_DiasHorariosDisponibilidade_HorarioDisponibilidadeId",
            "IX_HistoricoStatus_OrcamentoId",
            "IX_HistoricoStatus_OrdemServicoId",
            "IX_HistoricoStatus_TipoEntidade_EntidadeId",
            "IX_HorariosConsultores_HorarioDisponibilidadeId_PessoaId",
            "IX_HorariosConsultores_PessoaId",
            "IX_HorariosDisponibilidade_Hora",
            "IX_ItensServico_OrcamentoId",
            "IX_ItensServico_OrdemServicoId",
            "IX_ItensServico_PecaId",
            "IX_ItensServico_ServicoId",
            "IX_MotivosRecusaOrcamento_OrcamentoId",
            "IX_Orcamentos_AgendamentoId",
            "IX_Orcamentos_ConsultorId",
            "IX_Orcamentos_MecanicoId",
            "IX_Orcamentos_PessoaId",
            "IX_Orcamentos_VeiculoId",
            "IX_OrdensServico_ConsultorId",
            "IX_OrdensServico_MecanicoId",
            "IX_OrdensServico_PessoaId",
            "IX_OrdensServico_VeiculoId",
            "IX_Pecas_Codigo",
            "IX_Perfis_NomePerfil",
            "IX_PerfisPermissoes_PerfilId_PermissaoId",
            "IX_PerfisPermissoes_PermissaoId",
            "IX_Permissoes_Codigo",
            "IX_Pessoas_UsuarioId",
            "IX_UsuariosPerfis_PerfilId",
            "IX_UsuariosPerfis_UsuarioId_PerfilId",
            "IX_Veiculos_PessoaId",
            "IX_Veiculos_Placa"
        }, createIndexes);

        Assert.Contains(migrationBuilder.Operations, operation => operation is CreateTableOperation { Name: "Agendamentos" });
        Assert.Contains(migrationBuilder.Operations, operation => operation is CreateTableOperation { Name: "Orcamentos" });
        Assert.Contains(migrationBuilder.Operations, operation => operation is CreateIndexOperation { Name: "IX_Pessoas_UsuarioId" });
        Assert.Contains(migrationBuilder.Operations, operation => operation is CreateIndexOperation { Name: "IX_Veiculos_Placa" });
    }

    [Fact]
    public void Down_Deve_Remover_Tabelas_Na_Ordem_Esperada()
    {
        // Arrange
        var migration = new MigrationProxy();
        var migrationBuilder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        // Act
        migration.ApplyDown(migrationBuilder);

        // Assert
        var dropTables = migrationBuilder.Operations.OfType<DropTableOperation>().Select(operation => operation.Name).ToArray();

        Assert.Equal(21, migrationBuilder.Operations.Count);
        Assert.Equal(new[]
        {
            "Checklists",
            "DiasHorariosDisponibilidade",
            "HistoricoStatus",
            "HorariosConsultores",
            "ItensServico",
            "MotivosRecusaOrcamento",
            "PerfisPermissoes",
            "UsuariosPerfis",
            "OrdensServico",
            "Pecas",
            "Servicos",
            "Orcamentos",
            "Permissoes",
            "Perfis",
            "Agendamentos",
            "AgendaConsultor",
            "Veiculos",
            "DiasDisponibilidade",
            "HorariosDisponibilidade",
            "Pessoas",
            "Usuarios"
        }, dropTables);

        Assert.All(migrationBuilder.Operations, operation => Assert.IsType<DropTableOperation>(operation));
    }

    private sealed class MigrationProxy : _001inicial
    {
        public void ApplyUp(MigrationBuilder migrationBuilder)
        {
            base.Up(migrationBuilder);
        }

        public void ApplyDown(MigrationBuilder migrationBuilder)
        {
            base.Down(migrationBuilder);
        }
    }
}
