using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _008agendamentofks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
