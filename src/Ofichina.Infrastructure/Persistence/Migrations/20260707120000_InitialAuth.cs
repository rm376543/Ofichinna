using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Ofichina.Infrastructure.Persistence.Seeds;

#nullable disable

namespace Ofichina.Infrastructure.Persistence.Migrations;

public partial class InitialAuth : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Exemplos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Ativo = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Exemplos", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Perfis",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Descricao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                Ativo = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Perfis", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Usuarios",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                SenhaHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Ativo = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Usuarios", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UsuariosPerfis",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PerfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UsuariosPerfis", x => x.Id);
                table.ForeignKey(
                    name: "FK_UsuariosPerfis_Perfis_PerfilId",
                    column: x => x.PerfilId,
                    principalTable: "Perfis",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UsuariosPerfis_Usuarios_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "Usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "Perfis",
            columns: new[] { "Id", "Ativo", "Codigo", "CreatedAt", "Descricao", "Nome", "UpdatedAt" },
            values: new object[] { AuthSeed.AdminPerfilId, true, "ADMIN", new DateTime(2026, 7, 7, 12, 0, 0, DateTimeKind.Utc), "Perfil com acesso total ao sistema", "Administrador", null });

        migrationBuilder.InsertData(
            table: "Usuarios",
            columns: new[] { "Id", "Ativo", "CreatedAt", "Email", "Nome", "SenhaHash", "UpdatedAt" },
            values: new object[] { AuthSeed.AdminUsuarioId, true, new DateTime(2026, 7, 7, 12, 0, 0, DateTimeKind.Utc), "admin@ofichinna.local", "Administrador", AuthSeed.AdminPasswordHash, null });

        migrationBuilder.InsertData(
            table: "UsuariosPerfis",
            columns: new[] { "Id", "CreatedAt", "PerfilId", "UpdatedAt", "UsuarioId" },
            values: new object[] { Guid.Parse("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 7, 7, 12, 0, 0, DateTimeKind.Utc), AuthSeed.AdminPerfilId, null, AuthSeed.AdminUsuarioId });

        migrationBuilder.CreateIndex(
            name: "IX_Perfis_Codigo",
            table: "Perfis",
            column: "Codigo",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Usuarios_Email",
            table: "Usuarios",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_UsuariosPerfis_PerfilId",
            table: "UsuariosPerfis",
            column: "PerfilId");

        migrationBuilder.CreateIndex(
            name: "IX_UsuariosPerfis_UsuarioId_PerfilId",
            table: "UsuariosPerfis",
            columns: new[] { "UsuarioId", "PerfilId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Exemplos");

        migrationBuilder.DropTable(
            name: "UsuariosPerfis");

        migrationBuilder.DropTable(
            name: "Perfis");

        migrationBuilder.DropTable(
            name: "Usuarios");
    }
}