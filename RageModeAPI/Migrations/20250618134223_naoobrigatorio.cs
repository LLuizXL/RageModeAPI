using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RageModeAPI.Migrations
{
    /// <inheritdoc />
    public partial class naoobrigatorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Postagem_Personagens_PersonagemId",
                table: "Postagem");

            migrationBuilder.AlterColumn<Guid>(
                name: "PersonagemId",
                table: "Postagem",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Postagem_Personagens_PersonagemId",
                table: "Postagem",
                column: "PersonagemId",
                principalTable: "Personagens",
                principalColumn: "PersonagemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Postagem_Personagens_PersonagemId",
                table: "Postagem");

            migrationBuilder.AlterColumn<Guid>(
                name: "PersonagemId",
                table: "Postagem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Postagem_Personagens_PersonagemId",
                table: "Postagem",
                column: "PersonagemId",
                principalTable: "Personagens",
                principalColumn: "PersonagemId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
