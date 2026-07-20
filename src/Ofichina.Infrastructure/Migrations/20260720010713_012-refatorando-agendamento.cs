using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _012refatorandoagendamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DiaDisponibilidadeId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HorarioConsultorId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Agendamentos",
                type: "int",
                nullable: true);

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

            migrationBuilder.Sql(@"
INSERT INTO DiasDisponibilidade (Id, Data, CreatedAt, UpdatedAt, DeletedAt)
SELECT NEWID(), CONVERT(date, a.DataAgendamento), SYSUTCDATETIME(), NULL, NULL
FROM (SELECT DISTINCT DataAgendamento FROM Agendamentos) a
WHERE NOT EXISTS (
    SELECT 1
    FROM DiasDisponibilidade d
    WHERE d.Data = CONVERT(date, a.DataAgendamento)
);
" );

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

            migrationBuilder.Sql(@"
INSERT INTO HorariosDisponibilidade (Id, Hora, CreatedAt, UpdatedAt, DeletedAt)
SELECT NEWID(), CONVERT(time, a.HorarioAgendamento), SYSUTCDATETIME(), NULL, NULL
FROM (SELECT DISTINCT HorarioAgendamento FROM Agendamentos) a
WHERE NOT EXISTS (
    SELECT 1
    FROM HorariosDisponibilidade h
    WHERE h.Hora = CONVERT(time, a.HorarioAgendamento)
);
" );

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

            migrationBuilder.Sql(@"
INSERT INTO DiasHorariosDisponibilidade (Id, DiaDisponibilidadeId, HorarioDisponibilidadeId, CreatedAt, UpdatedAt, DeletedAt)
SELECT NEWID(), d.Id, h.Id, SYSUTCDATETIME(), NULL, NULL
FROM (
    SELECT DISTINCT DataAgendamento, HorarioAgendamento
    FROM Agendamentos
) a
INNER JOIN DiasDisponibilidade d ON d.Data = CONVERT(date, a.DataAgendamento)
INNER JOIN HorariosDisponibilidade h ON h.Hora = CONVERT(time, a.HorarioAgendamento)
WHERE NOT EXISTS (
    SELECT 1
    FROM DiasHorariosDisponibilidade dh
    WHERE dh.DiaDisponibilidadeId = d.Id
      AND dh.HorarioDisponibilidadeId = h.Id
);
" );

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

            migrationBuilder.Sql(@"
INSERT INTO HorariosConsultores (Id, HorarioDisponibilidadeId, PessoaId, CreatedAt, UpdatedAt, DeletedAt)
SELECT NEWID(), h.Id, a.ConsultorPessoaId, SYSUTCDATETIME(), NULL, NULL
FROM (
    SELECT DISTINCT ConsultorPessoaId, HorarioAgendamento
    FROM Agendamentos
) a
INNER JOIN HorariosDisponibilidade h ON h.Hora = CONVERT(time, a.HorarioAgendamento)
WHERE NOT EXISTS (
    SELECT 1
    FROM HorariosConsultores hc
    WHERE hc.HorarioDisponibilidadeId = h.Id
      AND hc.PessoaId = a.ConsultorPessoaId
);
" );

            migrationBuilder.Sql(@"
UPDATE a
SET DiaDisponibilidadeId = d.Id,
    HorarioConsultorId = hc.Id,
    Status = 1
FROM Agendamentos a
INNER JOIN DiasDisponibilidade d ON d.Data = CONVERT(date, a.DataAgendamento)
INNER JOIN HorariosDisponibilidade h ON h.Hora = CONVERT(time, a.HorarioAgendamento)
INNER JOIN HorariosConsultores hc ON hc.HorarioDisponibilidadeId = h.Id
    AND hc.PessoaId = a.ConsultorPessoaId;
" );

            migrationBuilder.Sql(@"
UPDATE Agendamentos
SET Status = 1
WHERE Status IS NULL;
" );

            migrationBuilder.AlterColumn<Guid>(
                name: "DiaDisponibilidadeId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "HorarioConsultorId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Agendamentos",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_DiasDisponibilidade_DiaDisponibilidadeId",
                table: "Agendamentos",
                column: "DiaDisponibilidadeId",
                principalTable: "DiasDisponibilidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_HorariosConsultores_HorarioConsultorId",
                table: "Agendamentos",
                column: "HorarioConsultorId",
                principalTable: "HorariosConsultores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_ConsultorPessoaId_DataAgendamento_HorarioAgendamento",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_DataAgendamento",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "DataAgendamento",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "HorarioAgendamento",
                table: "Agendamentos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_DiasDisponibilidade_DiaDisponibilidadeId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_HorariosConsultores_HorarioConsultorId",
                table: "Agendamentos");

            migrationBuilder.DropTable(
                name: "DiasHorariosDisponibilidade");

            migrationBuilder.DropTable(
                name: "HorariosConsultores");

            migrationBuilder.DropTable(
                name: "DiasDisponibilidade");

            migrationBuilder.DropTable(
                name: "HorariosDisponibilidade");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_DiaDisponibilidadeId",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_DiaDisponibilidadeId_HorarioConsultorId",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_HorarioConsultorId",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_Status",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "DiaDisponibilidadeId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "HorarioConsultorId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Agendamentos");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataAgendamento",
                table: "Agendamentos",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "HorarioAgendamento",
                table: "Agendamentos",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ConsultorPessoaId_DataAgendamento_HorarioAgendamento",
                table: "Agendamentos",
                columns: new[] { "ConsultorPessoaId", "DataAgendamento", "HorarioAgendamento" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_DataAgendamento",
                table: "Agendamentos",
                column: "DataAgendamento");
        }
    }
}
