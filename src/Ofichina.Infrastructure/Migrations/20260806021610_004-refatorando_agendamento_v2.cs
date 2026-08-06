using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _004refatorando_agendamento_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Checklists_ChecklistId",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_ChecklistId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "ChecklistId",
                table: "Agendamentos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChecklistId",
                table: "Agendamentos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ChecklistId",
                table: "Agendamentos",
                column: "ChecklistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Checklists_ChecklistId",
                table: "Agendamentos",
                column: "ChecklistId",
                principalTable: "Checklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
