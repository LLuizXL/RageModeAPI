using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RageModeAPI.Migrations
{
    /// <inheritdoc />
    public partial class parte2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Personagens",
                newName: "PersonagemNome");

            migrationBuilder.AddColumn<Guid>(
                name: "JogoId",
                table: "Personagens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "PersonagemDescricao",
                table: "Personagens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TipoPersonagemId",
                table: "Personagens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Jogos",
                columns: table => new
                {
                    JogosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JogoNome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JogoDescricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnoLancamento = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogos", x => x.JogosId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuariosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioNome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioSenha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuariosId);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    ComentariosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComentarioTexto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataComentario = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.ComentariosId);
                    table.ForeignKey(
                        name: "FK_Comentarios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuariosId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Postagem",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostTitulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostConteudo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoPost = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataPostagem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonagemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postagem", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Postagem_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "JogosId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Postagem_Personagens_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "Personagens",
                        principalColumn: "PersonagemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Postagem_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuariosId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seguidores",
                columns: table => new
                {
                    SeguidoresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeguidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seguidores", x => x.SeguidoresId);
                    table.ForeignKey(
                        name: "FK_Seguidores_Usuarios_SeguidoId",
                        column: x => x.SeguidoId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuariosId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seguidores_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuariosId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    LikesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsLike = table.Column<bool>(type: "bit", nullable: false),
                    UsuariosId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.LikesId);
                    table.ForeignKey(
                        name: "FK_Likes_Postagem_PostId",
                        column: x => x.PostId,
                        principalTable: "Postagem",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Usuarios_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuariosId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personagens_JogoId",
                table: "Personagens",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Personagens_TipoPersonagemId",
                table: "Personagens",
                column: "TipoPersonagemId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_PostId",
                table: "Likes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UsuariosId",
                table: "Likes",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_Postagem_JogoId",
                table: "Postagem",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Postagem_PersonagemId",
                table: "Postagem",
                column: "PersonagemId");

            migrationBuilder.CreateIndex(
                name: "IX_Postagem_UsuarioId",
                table: "Postagem",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Seguidores_SeguidoId",
                table: "Seguidores",
                column: "SeguidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Seguidores_UsuarioId",
                table: "Seguidores",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personagens_Jogos_JogoId",
                table: "Personagens",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "JogosId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Personagens_TiposPersonagens_TipoPersonagemId",
                table: "Personagens",
                column: "TipoPersonagemId",
                principalTable: "TiposPersonagens",
                principalColumn: "TipoPersonagemId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personagens_Jogos_JogoId",
                table: "Personagens");

            migrationBuilder.DropForeignKey(
                name: "FK_Personagens_TiposPersonagens_TipoPersonagemId",
                table: "Personagens");

            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Seguidores");

            migrationBuilder.DropTable(
                name: "Postagem");

            migrationBuilder.DropTable(
                name: "Jogos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Personagens_JogoId",
                table: "Personagens");

            migrationBuilder.DropIndex(
                name: "IX_Personagens_TipoPersonagemId",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "JogoId",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "PersonagemDescricao",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "TipoPersonagemId",
                table: "Personagens");

            migrationBuilder.RenameColumn(
                name: "PersonagemNome",
                table: "Personagens",
                newName: "Nome");
        }
    }
}
