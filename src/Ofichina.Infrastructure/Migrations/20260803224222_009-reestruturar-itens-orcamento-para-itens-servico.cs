using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _009reestruturaritensorcamentoparaitensservico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensOrcamentoPecas");

            migrationBuilder.DropTable(
                name: "ItensOrcamento");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdemServicoId",
                table: "ItensServico",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "OrcamentoId",
                table: "ItensServico",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrdemServicoId2",
                table: "ItensServico",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensServico_OrcamentoId",
                table: "ItensServico",
                column: "OrcamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensServico_OrdemServicoId2",
                table: "ItensServico",
                column: "OrdemServicoId2");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensServico_Orcamentos_OrcamentoId",
                table: "ItensServico",
                column: "OrcamentoId",
                principalTable: "Orcamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensServico_OrdensServico_OrdemServicoId2",
                table: "ItensServico",
                column: "OrdemServicoId2",
                principalTable: "OrdensServico",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensServico_Orcamentos_OrcamentoId",
                table: "ItensServico");

            migrationBuilder.DropForeignKey(
                name: "FK_ItensServico_OrdensServico_OrdemServicoId2",
                table: "ItensServico");

            migrationBuilder.DropIndex(
                name: "IX_ItensServico_OrcamentoId",
                table: "ItensServico");

            migrationBuilder.DropIndex(
                name: "IX_ItensServico_OrdemServicoId2",
                table: "ItensServico");

            migrationBuilder.DropColumn(
                name: "OrcamentoId",
                table: "ItensServico");

            migrationBuilder.DropColumn(
                name: "OrdemServicoId2",
                table: "ItensServico");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdemServicoId",
                table: "ItensServico",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ItensOrcamento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrcamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensOrcamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensOrcamento_Orcamentos_OrcamentoId",
                        column: x => x.OrcamentoId,
                        principalTable: "Orcamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensOrcamento_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensOrcamentoPecas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PecaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ItemOrcamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensOrcamentoPecas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensOrcamentoPecas_ItensOrcamento_ItemOrcamentoId",
                        column: x => x.ItemOrcamentoId,
                        principalTable: "ItensOrcamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensOrcamentoPecas_Pecas_PecaId",
                        column: x => x.PecaId,
                        principalTable: "Pecas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrcamento_OrcamentoId",
                table: "ItensOrcamento",
                column: "OrcamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrcamento_ServicoId",
                table: "ItensOrcamento",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrcamentoPecas_ItemOrcamentoId",
                table: "ItensOrcamentoPecas",
                column: "ItemOrcamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrcamentoPecas_PecaId",
                table: "ItensOrcamentoPecas",
                column: "PecaId");
        }
    }
}
