using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _013aninhandopecas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensPeca_OrdensServico_OrdemServicoId",
                table: "ItensPeca");

            migrationBuilder.RenameColumn(
                name: "OrdemServicoId",
                table: "ItensPeca",
                newName: "ItemServicoId");

            migrationBuilder.RenameIndex(
                name: "IX_ItensPeca_OrdemServicoId",
                table: "ItensPeca",
                newName: "IX_ItensPeca_ItemServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensPeca_ItensServico_ItemServicoId",
                table: "ItensPeca",
                column: "ItemServicoId",
                principalTable: "ItensServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensPeca_ItensServico_ItemServicoId",
                table: "ItensPeca");

            migrationBuilder.RenameColumn(
                name: "ItemServicoId",
                table: "ItensPeca",
                newName: "OrdemServicoId");

            migrationBuilder.RenameIndex(
                name: "IX_ItensPeca_ItemServicoId",
                table: "ItensPeca",
                newName: "IX_ItensPeca_OrdemServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensPeca_OrdensServico_OrdemServicoId",
                table: "ItensPeca",
                column: "OrdemServicoId",
                principalTable: "OrdensServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
