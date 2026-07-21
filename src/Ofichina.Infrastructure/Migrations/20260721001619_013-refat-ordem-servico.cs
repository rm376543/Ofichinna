using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _013refatordemservico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensPeca_ItensServico_ItemServicoId",
                table: "ItensPeca");

            migrationBuilder.DropForeignKey(
                name: "FK_ItensPeca_Pecas_PecaId",
                table: "ItensPeca");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItensPeca",
                table: "ItensPeca");

            migrationBuilder.RenameTable(
                name: "ItensPeca",
                newName: "PecaServico");

            migrationBuilder.RenameIndex(
                name: "IX_ItensPeca_PecaId",
                table: "PecaServico",
                newName: "IX_PecaServico_PecaId");

            migrationBuilder.RenameIndex(
                name: "IX_ItensPeca_ItemServicoId",
                table: "PecaServico",
                newName: "IX_PecaServico_ItemServicoId");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorUnitario",
                table: "PecaServico",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "PecaServico",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PecaServico",
                table: "PecaServico",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PecaServico_ItensServico_ItemServicoId",
                table: "PecaServico",
                column: "ItemServicoId",
                principalTable: "ItensServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PecaServico_Pecas_PecaId",
                table: "PecaServico",
                column: "PecaId",
                principalTable: "Pecas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PecaServico_ItensServico_ItemServicoId",
                table: "PecaServico");

            migrationBuilder.DropForeignKey(
                name: "FK_PecaServico_Pecas_PecaId",
                table: "PecaServico");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PecaServico",
                table: "PecaServico");

            migrationBuilder.RenameTable(
                name: "PecaServico",
                newName: "ItensPeca");

            migrationBuilder.RenameIndex(
                name: "IX_PecaServico_PecaId",
                table: "ItensPeca",
                newName: "IX_ItensPeca_PecaId");

            migrationBuilder.RenameIndex(
                name: "IX_PecaServico_ItemServicoId",
                table: "ItensPeca",
                newName: "IX_ItensPeca_ItemServicoId");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorUnitario",
                table: "ItensPeca",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "ItensPeca",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItensPeca",
                table: "ItensPeca",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensPeca_ItensServico_ItemServicoId",
                table: "ItensPeca",
                column: "ItemServicoId",
                principalTable: "ItensServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensPeca_Pecas_PecaId",
                table: "ItensPeca",
                column: "PecaId",
                principalTable: "Pecas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
