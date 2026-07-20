using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _011renomeandotabelas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemServico_OrdensServico_OrdemServicoId",
                table: "ItemServico");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemServico_Servicos_ServicoId",
                table: "ItemServico");

            migrationBuilder.DropForeignKey(
                name: "FK_PerfilPermissao_Perfil_PerfilId",
                table: "PerfilPermissao");

            migrationBuilder.DropForeignKey(
                name: "FK_PerfilPermissao_Permissao_PermissaoId",
                table: "PerfilPermissao");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Usuario_UsuarioId",
                table: "Pessoas");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioPerfil_Perfil_PerfilId",
                table: "UsuarioPerfil");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioPerfil_Usuario_UsuarioId",
                table: "UsuarioPerfil");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioPerfil",
                table: "UsuarioPerfil");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissao",
                table: "Permissao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PerfilPermissao",
                table: "PerfilPermissao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Perfil",
                table: "Perfil");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemServico",
                table: "ItemServico");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Pecas");

            migrationBuilder.RenameTable(
                name: "UsuarioPerfil",
                newName: "UsuariosPerfis");

            migrationBuilder.RenameTable(
                name: "Usuario",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Permissao",
                newName: "Permissoes");

            migrationBuilder.RenameTable(
                name: "PerfilPermissao",
                newName: "PerfisPermissoes");

            migrationBuilder.RenameTable(
                name: "Perfil",
                newName: "Perfis");

            migrationBuilder.RenameTable(
                name: "ItemServico",
                newName: "ItensServico");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioPerfil_UsuarioId_PerfilId",
                table: "UsuariosPerfis",
                newName: "IX_UsuariosPerfis_UsuarioId_PerfilId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioPerfil_PerfilId",
                table: "UsuariosPerfis",
                newName: "IX_UsuariosPerfis_PerfilId");

            migrationBuilder.RenameIndex(
                name: "IX_Permissao_Codigo",
                table: "Permissoes",
                newName: "IX_Permissoes_Codigo");

            migrationBuilder.RenameIndex(
                name: "IX_PerfilPermissao_PermissaoId",
                table: "PerfisPermissoes",
                newName: "IX_PerfisPermissoes_PermissaoId");

            migrationBuilder.RenameIndex(
                name: "IX_PerfilPermissao_PerfilId_PermissaoId",
                table: "PerfisPermissoes",
                newName: "IX_PerfisPermissoes_PerfilId_PermissaoId");

            migrationBuilder.RenameIndex(
                name: "IX_Perfil_NomePerfil",
                table: "Perfis",
                newName: "IX_Perfis_NomePerfil");

            migrationBuilder.RenameIndex(
                name: "IX_ItemServico_ServicoId",
                table: "ItensServico",
                newName: "IX_ItensServico_ServicoId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemServico_OrdemServicoId",
                table: "ItensServico",
                newName: "IX_ItensServico_OrdemServicoId");

            migrationBuilder.AlterColumn<Guid>(
                name: "PecaId",
                table: "ItensPeca",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuariosPerfis",
                table: "UsuariosPerfis",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissoes",
                table: "Permissoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PerfisPermissoes",
                table: "PerfisPermissoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Perfis",
                table: "Perfis",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItensServico",
                table: "ItensServico",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensServico_OrdensServico_OrdemServicoId",
                table: "ItensServico",
                column: "OrdemServicoId",
                principalTable: "OrdensServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensServico_Servicos_ServicoId",
                table: "ItensServico",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensServico_OrdensServico_OrdemServicoId",
                table: "ItensServico");

            migrationBuilder.DropForeignKey(
                name: "FK_ItensServico_Servicos_ServicoId",
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuariosPerfis",
                table: "UsuariosPerfis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissoes",
                table: "Permissoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PerfisPermissoes",
                table: "PerfisPermissoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Perfis",
                table: "Perfis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItensServico",
                table: "ItensServico");

            migrationBuilder.RenameTable(
                name: "UsuariosPerfis",
                newName: "UsuarioPerfil");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "Usuario");

            migrationBuilder.RenameTable(
                name: "Permissoes",
                newName: "Permissao");

            migrationBuilder.RenameTable(
                name: "PerfisPermissoes",
                newName: "PerfilPermissao");

            migrationBuilder.RenameTable(
                name: "Perfis",
                newName: "Perfil");

            migrationBuilder.RenameTable(
                name: "ItensServico",
                newName: "ItemServico");

            migrationBuilder.RenameIndex(
                name: "IX_UsuariosPerfis_UsuarioId_PerfilId",
                table: "UsuarioPerfil",
                newName: "IX_UsuarioPerfil_UsuarioId_PerfilId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuariosPerfis_PerfilId",
                table: "UsuarioPerfil",
                newName: "IX_UsuarioPerfil_PerfilId");

            migrationBuilder.RenameIndex(
                name: "IX_Permissoes_Codigo",
                table: "Permissao",
                newName: "IX_Permissao_Codigo");

            migrationBuilder.RenameIndex(
                name: "IX_PerfisPermissoes_PermissaoId",
                table: "PerfilPermissao",
                newName: "IX_PerfilPermissao_PermissaoId");

            migrationBuilder.RenameIndex(
                name: "IX_PerfisPermissoes_PerfilId_PermissaoId",
                table: "PerfilPermissao",
                newName: "IX_PerfilPermissao_PerfilId_PermissaoId");

            migrationBuilder.RenameIndex(
                name: "IX_Perfis_NomePerfil",
                table: "Perfil",
                newName: "IX_Perfil_NomePerfil");

            migrationBuilder.RenameIndex(
                name: "IX_ItensServico_ServicoId",
                table: "ItemServico",
                newName: "IX_ItemServico_ServicoId");

            migrationBuilder.RenameIndex(
                name: "IX_ItensServico_OrdemServicoId",
                table: "ItemServico",
                newName: "IX_ItemServico_OrdemServicoId");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Veiculos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Servicos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Pecas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "PecaId",
                table: "ItensPeca",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioPerfil",
                table: "UsuarioPerfil",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissao",
                table: "Permissao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PerfilPermissao",
                table: "PerfilPermissao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Perfil",
                table: "Perfil",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemServico",
                table: "ItemServico",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemServico_OrdensServico_OrdemServicoId",
                table: "ItemServico",
                column: "OrdemServicoId",
                principalTable: "OrdensServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemServico_Servicos_ServicoId",
                table: "ItemServico",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfilPermissao_Perfil_PerfilId",
                table: "PerfilPermissao",
                column: "PerfilId",
                principalTable: "Perfil",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfilPermissao_Permissao_PermissaoId",
                table: "PerfilPermissao",
                column: "PermissaoId",
                principalTable: "Permissao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Usuario_UsuarioId",
                table: "Pessoas",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioPerfil_Perfil_PerfilId",
                table: "UsuarioPerfil",
                column: "PerfilId",
                principalTable: "Perfil",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioPerfil_Usuario_UsuarioId",
                table: "UsuarioPerfil",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
