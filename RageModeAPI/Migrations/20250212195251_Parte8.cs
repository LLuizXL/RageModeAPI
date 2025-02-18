using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RageModeAPI.Migrations
{
    /// <inheritdoc />
    public partial class Parte8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personagens_Jogos_JogosId",
                table: "Personagens");

            migrationBuilder.AlterColumn<Guid>(
                name: "JogosId",
                table: "Personagens",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "PersonagemId",
                table: "Jogos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_PersonagemId",
                table: "Jogos",
                column: "PersonagemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Personagens_PersonagemId",
                table: "Jogos",
                column: "PersonagemId",
                principalTable: "Personagens",
                principalColumn: "PersonagemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Personagens_Jogos_JogosId",
                table: "Personagens",
                column: "JogosId",
                principalTable: "Jogos",
                principalColumn: "JogosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Personagens_PersonagemId",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Personagens_Jogos_JogosId",
                table: "Personagens");

            migrationBuilder.DropIndex(
                name: "IX_Jogos_PersonagemId",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "PersonagemId",
                table: "Jogos");

            migrationBuilder.AlterColumn<Guid>(
                name: "JogosId",
                table: "Personagens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Personagens_Jogos_JogosId",
                table: "Personagens",
                column: "JogosId",
                principalTable: "Jogos",
                principalColumn: "JogosId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
