using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _002pecaservicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PecaServico_ServicoId_PecaId",
                table: "PecaServico",
                columns: new[] { "ServicoId", "PecaId" },
                unique: true,
                filter: "[DeletedAt] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PecaServico_ServicoId_PecaId",
                table: "PecaServico");
        }
    }
}
