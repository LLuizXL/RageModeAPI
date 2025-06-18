using RageModeAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace RageModeAPI.Data
{
    public class RageModeApiContext : IdentityDbContext<Usuarios, IdentityRole, string> // Especifique explicitamente seus Usuarios como o Usuário Identity
    {
        //Método construtor
        public RageModeApiContext(DbContextOptions<RageModeApiContext> options) : base(options)
        {
        }

        public DbSet<Jogos> Jogos { get; set; }
        public DbSet<Personagem> Personagens { get; set; }
        public DbSet<TipoPersonagem> TiposPersonagens { get; set; }
        public DbSet<Comentarios> Comentarios { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; } // tabela de usuário personalizada
        public DbSet<Seguidores> Seguidores { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Likes> Likes { get; set; }

        //Sobrescrever o método OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // CRUCIAL: Chame o método base para IdentityDbContext

            // Mapeamentos de Tabela (Opcional, mas bom para nomes de tabela personalizados)
            modelBuilder.Entity<Personagem>().ToTable("Personagens");
            modelBuilder.Entity<TipoPersonagem>().ToTable("TiposPersonagens");
            modelBuilder.Entity<Jogos>().ToTable("Jogos");
            modelBuilder.Entity<Post>().ToTable("Postagem");
            modelBuilder.Entity<Comentarios>().ToTable("Comentarios");
            modelBuilder.Entity<Usuarios>().ToTable("Usuarios"); // Mapeia IdentityUser para a tabela 'Usuarios'
            modelBuilder.Entity<Likes>().ToTable("Likes");

            // --- Configurar Relacionamentos ---

            // Configuração dos relacionamentos de Follow (Seguidores)
            modelBuilder.Entity<Seguidores>()
                .HasOne(f => f.Usuario) // O usuário que segue
                .WithMany(u => u.Seguindo)
                .HasForeignKey(f => f.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict); // RESTRICT: Previne ciclos

            modelBuilder.Entity<Seguidores>()
                .HasOne(f => f.Seguido) // O usuário que está sendo seguido
                .WithMany(u => u.Seguidores)
                .HasForeignKey(f => f.SeguidoId)
                .OnDelete(DeleteBehavior.Restrict); // RESTRICT: Previne ciclos

            // Configuração para Post -> Author (Usuario)
            // *** ATENÇÃO: Ajuste aqui conforme sua escolha na model Post.cs ***
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Usuarios) // Se você escolheu Opção B (Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UsuarioId) // Se você escolheu Opção B (AuthorId)
                .OnDelete(DeleteBehavior.Restrict); // MANTENHA RESTRICT AQUI para evitar o erro de ciclo

            // Se você escolheu a Opção A (UsuarioId/Usuarios) na model Post, use ISSO ABAIXO:
            // modelBuilder.Entity<Post>()
            //     .HasOne(p => p.Usuarios)
            //     .WithMany(u => u.Posts)
            //     .HasForeignKey(p => p.UsuarioId)
            //     .OnDelete(DeleteBehavior.Restrict);


            // Configuração para Post -> Personagem
            //modelBuilder.Entity<Post>()
            //    .HasOne(p => p.Personagem)
            //    .WithMany(per => per.Posts) // Assumindo ICollection<Post> Posts em Personagem
            //    .HasForeignKey(p => p.PersonagemId)
            //    .OnDelete(DeleteBehavior.Restrict); // Geralmente RESTRICT para lookups

            // Configuração para Likes -> Usuarios
            modelBuilder.Entity<Likes>()
     .HasOne(l => l.Usuarios)
     .WithMany(u => u.Likes)
     .HasForeignKey(l => l.UsuariosId)
     .OnDelete(DeleteBehavior.Restrict); // MANTENHA RESTRICT AQUI para evitar o erro de ciclo

            // Configuração para Likes -> Post
            modelBuilder.Entity<Likes>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade); // CASCADE: Se o Post é excluído, os Likes dele são excluídos.

            // Configuração para Comentarios -> Usuario
            modelBuilder.Entity<Comentarios>()
      .HasOne(c => c.Usuario)
      .WithMany(u => u.Comentarios)
      .HasForeignKey(c => c.UsuarioId)
      .OnDelete(DeleteBehavior.Cascade);

            // Configuração para Comentarios -> Post
            modelBuilder.Entity<Comentarios>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comentarios)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade); // CASCADE: Se o Post é excluído, os Comentários dele são excluídos.

            // Configuração para Personagem -> TipoPersonagem
            modelBuilder.Entity<Personagem>()
                .HasOne(p => p.TipoPersonagem)
                .WithMany(tp => tp.Personagens) // Assumindo ICollection<Personagem> Personagens em TipoPersonagem
                .HasForeignKey(p => p.TipoPersonagemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração para Personagem -> Jogos
            modelBuilder.Entity<Personagem>()
          .HasOne(p => p.Jogo)
          .WithMany(j => j.Personagens)
          .HasForeignKey(p => p.JogoId)
          .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
    .HasOne(p => p.Usuarios)
    .WithMany(u => u.Posts)
    .HasForeignKey(p => p.UsuarioId)
    .OnDelete(DeleteBehavior.Cascade);


        }
    }
}