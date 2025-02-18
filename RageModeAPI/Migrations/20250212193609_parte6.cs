using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RageModeAPI.Migrations
{
    /// <inheritdoc />
    public partial class parte6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personagens_Jogos_JogosId",
                table: "Personagens");

            migrationBuilder.AddForeignKey(
                name: "FK_Personagens_Jogos_JogosId",
                table: "Personagens",
                column: "JogosId",
                principalTable: "Jogos",
                principalColumn: "JogosId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personagens_Jogos_JogosId",
                table: "Personagens");

            migrationBuilder.AddForeignKey(
                name: "FK_Personagens_Jogos_JogosId",
                table: "Personagens",
                column: "JogosId",
                principalTable: "Jogos",
                principalColumn: "JogosId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
