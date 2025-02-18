using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RageModeAPI.Migrations
{
    /// <inheritdoc />
    public partial class teste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Usuarios",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Usuarios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_UserId1",
                table: "Usuarios",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_AspNetUsers_UserId1",
                table: "Usuarios",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_AspNetUsers_UserId1",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_UserId1",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Usuarios");
        }
    }
}
