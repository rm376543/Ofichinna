using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _004orcamentoservicospecas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensOrcamento_Pecas_PecaId",
                table: "ItensOrcamento");

            migrationBuilder.DropIndex(
                name: "IX_ItensOrcamento_PecaId",
                table: "ItensOrcamento");

            migrationBuilder.DropColumn(
                name: "PecaId",
                table: "ItensOrcamento");

            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "ItensOrcamento");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServicoId",
                table: "ItensOrcamento",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ItensOrcamentoPecas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemOrcamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PecaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensOrcamentoPecas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensOrcamentoPecas_ItensOrcamento_ItemOrcamentoId",
                        column: x => x.ItemOrcamentoId,
                        principalTable: "ItensOrcamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensOrcamentoPecas_Pecas_PecaId",
                        column: x => x.PecaId,
                        principalTable: "Pecas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrcamentoPecas_ItemOrcamentoId",
                table: "ItensOrcamentoPecas",
                column: "ItemOrcamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrcamentoPecas_PecaId",
                table: "ItensOrcamentoPecas",
                column: "PecaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensOrcamentoPecas");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServicoId",
                table: "ItensOrcamento",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "PecaId",
                table: "ItensOrcamento",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantidade",
                table: "ItensOrcamento",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrcamento_PecaId",
                table: "ItensOrcamento",
                column: "PecaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensOrcamento_Pecas_PecaId",
                table: "ItensOrcamento",
                column: "PecaId",
                principalTable: "Pecas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
