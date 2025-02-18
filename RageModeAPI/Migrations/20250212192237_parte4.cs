using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RageModeAPI.Migrations
{
    /// <inheritdoc />
    public partial class parte4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personagens_Jogos_JogoId",
                table: "Personagens");

            migrationBuilder.RenameColumn(
                name: "JogoId",
                table: "Personagens",
                newName: "JogosId");

            migrationBuilder.RenameIndex(
                name: "IX_Personagens_JogoId",
                table: "Personagens",
                newName: "IX_Personagens_JogosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personagens_Jogos_JogosId",
                table: "Personagens",
                column: "JogosId",
                principalTable: "Jogos",
                principalColumn: "JogosId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personagens_Jogos_JogosId",
                table: "Personagens");

            migrationBuilder.RenameColumn(
                name: "JogosId",
                table: "Personagens",
                newName: "JogoId");

            migrationBuilder.RenameIndex(
                name: "IX_Personagens_JogosId",
                table: "Personagens",
                newName: "IX_Personagens_JogoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personagens_Jogos_JogoId",
                table: "Personagens",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "JogosId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
