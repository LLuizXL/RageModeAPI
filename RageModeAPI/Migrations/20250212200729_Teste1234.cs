using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RageModeAPI.Migrations
{
    /// <inheritdoc />
    public partial class Teste1234 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Usuarios_UsuariosId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Postagem_Jogos_JogoId",
                table: "Postagem");

            migrationBuilder.DropForeignKey(
                name: "FK_Postagem_Usuarios_UsuarioId",
                table: "Postagem");

            migrationBuilder.DropIndex(
                name: "IX_Postagem_JogoId",
                table: "Postagem");

            migrationBuilder.DropIndex(
                name: "IX_Postagem_UsuarioId",
                table: "Postagem");

            migrationBuilder.DropColumn(
                name: "JogoId",
                table: "Postagem");

            migrationBuilder.RenameColumn(
                name: "IsLike",
                table: "Likes",
                newName: "LikeorNot");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuariosId",
                table: "Postagem",
                type: "uniqueidentifier",
                nullable: true);

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
                name: "IX_Postagem_UsuariosId",
                table: "Postagem",
                column: "UsuariosId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Postagem_Usuarios_UsuariosId",
                table: "Postagem",
                column: "UsuariosId",
                principalTable: "Usuarios",
                principalColumn: "UsuariosId");
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

            migrationBuilder.DropForeignKey(
                name: "FK_Postagem_Usuarios_UsuariosId",
                table: "Postagem");

            migrationBuilder.DropIndex(
                name: "IX_Postagem_UsuariosId",
                table: "Postagem");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_PostId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "UsuariosId",
                table: "Postagem");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Comentarios");

            migrationBuilder.RenameColumn(
                name: "LikeorNot",
                table: "Likes",
                newName: "IsLike");

            migrationBuilder.AddColumn<Guid>(
                name: "JogoId",
                table: "Postagem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuariosId",
                table: "Likes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Postagem_JogoId",
                table: "Postagem",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Postagem_UsuarioId",
                table: "Postagem",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Usuarios_UsuariosId",
                table: "Likes",
                column: "UsuariosId",
                principalTable: "Usuarios",
                principalColumn: "UsuariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Postagem_Jogos_JogoId",
                table: "Postagem",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "JogosId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Postagem_Usuarios_UsuarioId",
                table: "Postagem",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuariosId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
