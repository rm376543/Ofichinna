using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _007itempecapeca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PecaId",
                table: "ItensPeca",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_ItensPeca_PecaId",
                table: "ItensPeca",
                column: "PecaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensPeca_Pecas_PecaId",
                table: "ItensPeca",
                column: "PecaId",
                principalTable: "Pecas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensPeca_Pecas_PecaId",
                table: "ItensPeca");

            migrationBuilder.DropIndex(
                name: "IX_ItensPeca_PecaId",
                table: "ItensPeca");

            migrationBuilder.DropColumn(
                name: "PecaId",
                table: "ItensPeca");
        }
    }
}
