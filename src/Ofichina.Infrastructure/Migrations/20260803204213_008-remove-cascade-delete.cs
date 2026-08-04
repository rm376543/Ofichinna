using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _008removecascadedelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensServico_OrdensServico_OrdemServicoId",
                table: "ItensServico");

            migrationBuilder.DropForeignKey(
                name: "FK_PerfisPermissoes_Perfis_PerfilId",
                table: "PerfisPermissoes");

            migrationBuilder.DropForeignKey(
                name: "FK_PerfisPermissoes_Permissoes_PermissaoId",
                table: "PerfisPermissoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Usuarios_UsuarioId",
                table: "Pessoas");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosPerfis_Perfis_PerfilId",
                table: "UsuariosPerfis");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosPerfis_Usuarios_UsuarioId",
                table: "UsuariosPerfis");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiculos_Pessoas_PessoaId",
                table: "Veiculos");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensServico_OrdensServico_OrdemServicoId",
                table: "ItensServico",
                column: "OrdemServicoId",
                principalTable: "OrdensServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfisPermissoes_Perfis_PerfilId",
                table: "PerfisPermissoes",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfisPermissoes_Permissoes_PermissaoId",
                table: "PerfisPermissoes",
                column: "PermissaoId",
                principalTable: "Permissoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Usuarios_UsuarioId",
                table: "Pessoas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosPerfis_Perfis_PerfilId",
                table: "UsuariosPerfis",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosPerfis_Usuarios_UsuarioId",
                table: "UsuariosPerfis",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculos_Pessoas_PessoaId",
                table: "Veiculos",
                column: "PessoaId",
                principalTable: "Pessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensServico_OrdensServico_OrdemServicoId",
                table: "ItensServico");

            migrationBuilder.DropForeignKey(
                name: "FK_PerfisPermissoes_Perfis_PerfilId",
                table: "PerfisPermissoes");

            migrationBuilder.DropForeignKey(
                name: "FK_PerfisPermissoes_Permissoes_PermissaoId",
                table: "PerfisPermissoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Usuarios_UsuarioId",
                table: "Pessoas");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosPerfis_Perfis_PerfilId",
                table: "UsuariosPerfis");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosPerfis_Usuarios_UsuarioId",
                table: "UsuariosPerfis");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiculos_Pessoas_PessoaId",
                table: "Veiculos");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensServico_OrdensServico_OrdemServicoId",
                table: "ItensServico",
                column: "OrdemServicoId",
                principalTable: "OrdensServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfisPermissoes_Perfis_PerfilId",
                table: "PerfisPermissoes",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfisPermissoes_Permissoes_PermissaoId",
                table: "PerfisPermissoes",
                column: "PermissaoId",
                principalTable: "Permissoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Usuarios_UsuarioId",
                table: "Pessoas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosPerfis_Perfis_PerfilId",
                table: "UsuariosPerfis",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosPerfis_Usuarios_UsuarioId",
                table: "UsuariosPerfis",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculos_Pessoas_PessoaId",
                table: "Veiculos",
                column: "PessoaId",
                principalTable: "Pessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
