using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _001inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiasDisponibilidade",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiasDisponibilidade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HorariosDisponibilidade",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosDisponibilidade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pecas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    QuantidadeEstoque = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pecas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Perfis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomePerfil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiasHorariosDisponibilidade",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaDisponibilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HorarioDisponibilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiasHorariosDisponibilidade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiasHorariosDisponibilidade_DiasDisponibilidade_DiaDisponibilidadeId",
                        column: x => x.DiaDisponibilidadeId,
                        principalTable: "DiasDisponibilidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiasHorariosDisponibilidade_HorariosDisponibilidade_HorarioDisponibilidadeId",
                        column: x => x.HorarioDisponibilidadeId,
                        principalTable: "HorariosDisponibilidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfisPermissoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisPermissoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfisPermissoes_Perfis_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfisPermissoes_Permissoes_PermissaoId",
                        column: x => x.PermissaoId,
                        principalTable: "Permissoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PecaServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PecaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Utilizada = table.Column<bool>(type: "bit", nullable: false),
                    DataUtilizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PecaServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PecaServico_Pecas_PecaId",
                        column: x => x.PecaId,
                        principalTable: "Pecas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PecaServico_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pessoas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    EnderecoLogradouro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EnderecoNumero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EnderecoComplemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EnderecoBairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EnderecoCidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EnderecoEstado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    EnderecoCep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pessoas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosPerfis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosPerfis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosPerfis_Perfis_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosPerfis_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HorariosConsultores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HorarioDisponibilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosConsultores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorariosConsultores_HorariosDisponibilidade_HorarioDisponibilidadeId",
                        column: x => x.HorarioDisponibilidadeId,
                        principalTable: "HorariosDisponibilidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HorariosConsultores_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnoFabricacao = table.Column<int>(type: "int", nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Hodometro = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veiculos_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientePessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaDisponibilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HorarioConsultorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultorPessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VeiculoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agendamentos_DiasDisponibilidade_DiaDisponibilidadeId",
                        column: x => x.DiaDisponibilidadeId,
                        principalTable: "DiasDisponibilidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agendamentos_HorariosConsultores_HorarioConsultorId",
                        column: x => x.HorarioConsultorId,
                        principalTable: "HorariosConsultores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Pessoas_ClientePessoaId",
                        column: x => x.ClientePessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Pessoas_ConsultorPessoaId",
                        column: x => x.ConsultorPessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdensServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VeiculoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FuncionarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HodometroEntrada = table.Column<int>(type: "int", nullable: false),
                    ProblemaRelatado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFinalizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Pessoas_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PecaServicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensServico_OrdensServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensServico_PecaServico_PecaServicoId",
                        column: x => x.PecaServicoId,
                        principalTable: "PecaServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ClientePessoaId",
                table: "Agendamentos",
                column: "ClientePessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ConsultorPessoaId",
                table: "Agendamentos",
                column: "ConsultorPessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_DiaDisponibilidadeId",
                table: "Agendamentos",
                column: "DiaDisponibilidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_DiaDisponibilidadeId_HorarioConsultorId",
                table: "Agendamentos",
                columns: new[] { "DiaDisponibilidadeId", "HorarioConsultorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_HorarioConsultorId",
                table: "Agendamentos",
                column: "HorarioConsultorId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_Status",
                table: "Agendamentos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_VeiculoId",
                table: "Agendamentos",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_DiasDisponibilidade_Data",
                table: "DiasDisponibilidade",
                column: "Data",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiasHorariosDisponibilidade_DiaDisponibilidadeId_HorarioDisponibilidadeId",
                table: "DiasHorariosDisponibilidade",
                columns: new[] { "DiaDisponibilidadeId", "HorarioDisponibilidadeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiasHorariosDisponibilidade_HorarioDisponibilidadeId",
                table: "DiasHorariosDisponibilidade",
                column: "HorarioDisponibilidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosConsultores_HorarioDisponibilidadeId_PessoaId",
                table: "HorariosConsultores",
                columns: new[] { "HorarioDisponibilidadeId", "PessoaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HorariosConsultores_PessoaId",
                table: "HorariosConsultores",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosDisponibilidade_Hora",
                table: "HorariosDisponibilidade",
                column: "Hora",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensServico_OrdemServicoId",
                table: "ItensServico",
                column: "OrdemServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensServico_PecaServicoId",
                table: "ItensServico",
                column: "PecaServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_FuncionarioId",
                table: "OrdensServico",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_PessoaId",
                table: "OrdensServico",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_VeiculoId",
                table: "OrdensServico",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pecas_Codigo",
                table: "Pecas",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PecaServico_PecaId",
                table: "PecaServico",
                column: "PecaId");

            migrationBuilder.CreateIndex(
                name: "IX_PecaServico_ServicoId",
                table: "PecaServico",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Perfis_NomePerfil",
                table: "Perfis",
                column: "NomePerfil",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfisPermissoes_PerfilId_PermissaoId",
                table: "PerfisPermissoes",
                columns: new[] { "PerfilId", "PermissaoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfisPermissoes_PermissaoId",
                table: "PerfisPermissoes",
                column: "PermissaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissoes_Codigo",
                table: "Permissoes",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_UsuarioId",
                table: "Pessoas",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPerfis_PerfilId",
                table: "UsuariosPerfis",
                column: "PerfilId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPerfis_UsuarioId_PerfilId",
                table: "UsuariosPerfis",
                columns: new[] { "UsuarioId", "PerfilId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_PessoaId",
                table: "Veiculos",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_Placa",
                table: "Veiculos",
                column: "Placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "DiasHorariosDisponibilidade");

            migrationBuilder.DropTable(
                name: "ItensServico");

            migrationBuilder.DropTable(
                name: "PerfisPermissoes");

            migrationBuilder.DropTable(
                name: "UsuariosPerfis");

            migrationBuilder.DropTable(
                name: "HorariosConsultores");

            migrationBuilder.DropTable(
                name: "DiasDisponibilidade");

            migrationBuilder.DropTable(
                name: "OrdensServico");

            migrationBuilder.DropTable(
                name: "PecaServico");

            migrationBuilder.DropTable(
                name: "Permissoes");

            migrationBuilder.DropTable(
                name: "Perfis");

            migrationBuilder.DropTable(
                name: "HorariosDisponibilidade");

            migrationBuilder.DropTable(
                name: "Veiculos");

            migrationBuilder.DropTable(
                name: "Pecas");

            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropTable(
                name: "Pessoas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
