using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RageModeAPI.Migrations
{
    /// <inheritdoc />
    public partial class Teste123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Usuarios_UsuariosId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "IsLike",
                table: "Likes",
                newName: "LikeorNot");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuariosId",
                table: "Likes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "Comentarios",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_PostId",
                table: "Comentarios",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Postagem_PostId",
                table: "Comentarios",
                column: "PostId",
                principalTable: "Postagem",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Usuarios_UsuariosId",
                table: "Likes",
                column: "UsuariosId",
                principalTable: "Usuarios",
                principalColumn: "UsuariosId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Postagem_PostId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Usuarios_UsuariosId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_PostId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Comentarios");

            migrationBuilder.RenameColumn(
                name: "LikeorNot",
                table: "Likes",
                newName: "IsLike");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuariosId",
                table: "Likes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Usuarios_UsuariosId",
                table: "Likes",
                column: "UsuariosId",
                principalTable: "Usuarios",
                principalColumn: "UsuariosId");
        }
    }
}
