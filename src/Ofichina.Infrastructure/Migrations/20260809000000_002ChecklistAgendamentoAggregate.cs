using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ofichina.Infrastructure.Migrations;

/// <inheritdoc />
public partial class _002ChecklistAgendamentoAggregate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Checklists_Pessoas_PessoaId",
            table: "Checklists");

        migrationBuilder.DropForeignKey(
            name: "FK_Checklists_Veiculos_VeiculoId",
            table: "Checklists");

        migrationBuilder.DropIndex(
            name: "IX_Checklists_PessoaId",
            table: "Checklists");

        migrationBuilder.DropIndex(
            name: "IX_Checklists_VeiculoId",
            table: "Checklists");

        migrationBuilder.DropColumn(
            name: "HodometroEntrada",
            table: "Checklists");

        migrationBuilder.DropColumn(
            name: "PessoaId",
            table: "Checklists");

        migrationBuilder.DropColumn(
            name: "VeiculoId",
            table: "Checklists");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "HodometroEntrada",
            table: "Checklists",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<Guid>(
            name: "PessoaId",
            table: "Checklists",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "VeiculoId",
            table: "Checklists",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateIndex(
            name: "IX_Checklists_PessoaId",
            table: "Checklists",
            column: "PessoaId");

        migrationBuilder.CreateIndex(
            name: "IX_Checklists_VeiculoId",
            table: "Checklists",
            column: "VeiculoId");

        migrationBuilder.AddForeignKey(
            name: "FK_Checklists_Pessoas_PessoaId",
            table: "Checklists",
            column: "PessoaId",
            principalTable: "Pessoas",
            principalColumn: "PessoaId",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Checklists_Veiculos_VeiculoId",
            table: "Checklists",
            column: "VeiculoId",
            principalTable: "Veiculos",
            principalColumn: "VeiculoId",
            onDelete: ReferentialAction.Restrict);
    }
}