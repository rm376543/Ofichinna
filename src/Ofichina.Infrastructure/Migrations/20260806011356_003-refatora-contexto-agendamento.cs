using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _003refatoracontextoagendamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_DiaDisponibilidadeId_HorarioConsultorId",
                table: "Agendamentos");

            migrationBuilder.AddColumn<Guid>(
                name: "AgendamentoId",
                table: "Checklists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "HorarioConsultorId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DiaDisponibilidadeId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConsultorPessoaId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ChecklistId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HorarioConsultorDisponibilidadeId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "HorariosConsultorDisponibilidade",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaDisponibilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HorarioDisponibilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultorPessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosConsultorDisponibilidade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorariosConsultorDisponibilidade_DiasDisponibilidade_DiaDisponibilidadeId",
                        column: x => x.DiaDisponibilidadeId,
                        principalTable: "DiasDisponibilidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HorariosConsultorDisponibilidade_HorariosDisponibilidade_HorarioDisponibilidadeId",
                        column: x => x.HorarioDisponibilidadeId,
                        principalTable: "HorariosDisponibilidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HorariosConsultorDisponibilidade_Pessoas_ConsultorPessoaId",
                        column: x => x.ConsultorPessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_AgendamentoId",
                table: "Checklists",
                column: "AgendamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ChecklistId",
                table: "Agendamentos",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_HorarioConsultorDisponibilidadeId",
                table: "Agendamentos",
                column: "HorarioConsultorDisponibilidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosConsultorDisponibilidade_ConsultorPessoaId",
                table: "HorariosConsultorDisponibilidade",
                column: "ConsultorPessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosConsultorDisponibilidade_DiaDisponibilidadeId",
                table: "HorariosConsultorDisponibilidade",
                column: "DiaDisponibilidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosConsultorDisponibilidade_DiaHorarioConsultor",
                table: "HorariosConsultorDisponibilidade",
                columns: new[] { "DiaDisponibilidadeId", "HorarioDisponibilidadeId", "ConsultorPessoaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HorariosConsultorDisponibilidade_HorarioDisponibilidadeId",
                table: "HorariosConsultorDisponibilidade",
                column: "HorarioDisponibilidadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Checklists_ChecklistId",
                table: "Agendamentos",
                column: "ChecklistId",
                principalTable: "Checklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_HorariosConsultorDisponibilidade_HorarioConsultorDisponibilidadeId",
                table: "Agendamentos",
                column: "HorarioConsultorDisponibilidadeId",
                principalTable: "HorariosConsultorDisponibilidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Checklists_ChecklistId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_HorariosConsultorDisponibilidade_HorarioConsultorDisponibilidadeId",
                table: "Agendamentos");

            migrationBuilder.DropTable(
                name: "HorariosConsultorDisponibilidade");

            migrationBuilder.DropIndex(
                name: "IX_Checklists_AgendamentoId",
                table: "Checklists");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_ChecklistId",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_HorarioConsultorDisponibilidadeId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "AgendamentoId",
                table: "Checklists");

            migrationBuilder.DropColumn(
                name: "ChecklistId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "HorarioConsultorDisponibilidadeId",
                table: "Agendamentos");

            migrationBuilder.AlterColumn<Guid>(
                name: "HorarioConsultorId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DiaDisponibilidadeId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ConsultorPessoaId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_DiaDisponibilidadeId_HorarioConsultorId",
                table: "Agendamentos",
                columns: new[] { "DiaDisponibilidadeId", "HorarioConsultorId" },
                unique: true);
        }
    }
}
