using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _006agendamentonovofluxo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaDisponibilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HorarioConsultorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VeiculoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DescricaoProblema = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CanalAtendimento = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Pessoas_PessoaId",
                        column: x => x.PessoaId,
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

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_DiaDisponibilidadeId_HorarioConsultorId",
                table: "Agendamentos",
                columns: new[] { "DiaDisponibilidadeId", "HorarioConsultorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_PessoaId",
                table: "Agendamentos",
                column: "PessoaId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "DiasHorariosDisponibilidade");

            migrationBuilder.DropTable(
                name: "HorariosConsultores");

            migrationBuilder.DropTable(
                name: "DiasDisponibilidade");

            migrationBuilder.DropTable(
                name: "HorariosDisponibilidade");
        }
    }
}
