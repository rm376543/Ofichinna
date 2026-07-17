using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _004itemservicoservicoid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServicoId",
                table: "ItemServico",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_ItemServico_ServicoId",
                table: "ItemServico",
                column: "ServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemServico_Servicos_ServicoId",
                table: "ItemServico",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemServico_Servicos_ServicoId",
                table: "ItemServico");

            migrationBuilder.DropIndex(
                name: "IX_ItemServico_ServicoId",
                table: "ItemServico");

            migrationBuilder.DropColumn(
                name: "ServicoId",
                table: "ItemServico");
        }
    }
}
