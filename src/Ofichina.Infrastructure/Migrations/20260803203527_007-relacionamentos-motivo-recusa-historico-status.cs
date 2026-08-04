using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _007relacionamentosmotivorecusahistoricostatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrcamentoId",
                table: "HistoricoStatus",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrdemServicoId",
                table: "HistoricoStatus",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoStatus_OrcamentoId",
                table: "HistoricoStatus",
                column: "OrcamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoStatus_OrdemServicoId",
                table: "HistoricoStatus",
                column: "OrdemServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricoStatus_Orcamentos_OrcamentoId",
                table: "HistoricoStatus",
                column: "OrcamentoId",
                principalTable: "Orcamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricoStatus_OrdensServico_OrdemServicoId",
                table: "HistoricoStatus",
                column: "OrdemServicoId",
                principalTable: "OrdensServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MotivosRecusaOrcamento_Orcamentos_OrcamentoId",
                table: "MotivosRecusaOrcamento",
                column: "OrcamentoId",
                principalTable: "Orcamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricoStatus_Orcamentos_OrcamentoId",
                table: "HistoricoStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoricoStatus_OrdensServico_OrdemServicoId",
                table: "HistoricoStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_MotivosRecusaOrcamento_Orcamentos_OrcamentoId",
                table: "MotivosRecusaOrcamento");

            migrationBuilder.DropIndex(
                name: "IX_HistoricoStatus_OrcamentoId",
                table: "HistoricoStatus");

            migrationBuilder.DropIndex(
                name: "IX_HistoricoStatus_OrdemServicoId",
                table: "HistoricoStatus");

            migrationBuilder.DropColumn(
                name: "OrcamentoId",
                table: "HistoricoStatus");

            migrationBuilder.DropColumn(
                name: "OrdemServicoId",
                table: "HistoricoStatus");
        }
    }
}
