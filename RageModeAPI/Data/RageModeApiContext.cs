using RageModeAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RageModeAPI.Data
{
    public class RageModeApiContext : IdentityDbContext
    {
        //Método construtor
        public RageModeApiContext(DbContextOptions<RageModeApiContext> options) : base(options)
        {
        }

        public DbSet<Jogos> Jogos { get; set; }
        public DbSet<Personagem> Personagens { get; set; }
        public DbSet<TipoPersonagem> TiposPersonagens { get; set; }
        public DbSet<Comentarios> Comentarios { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Seguidores> Seguidores { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Likes> Likes { get; set; }
        //Sobrescrever o método OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Personagem>().ToTable("Personagens");
            modelBuilder.Entity<TipoPersonagem>().ToTable("TiposPersonagens");
            modelBuilder.Entity<Jogos>().ToTable("Jogos");
            modelBuilder.Entity<Post>().ToTable("Postagem");
            modelBuilder.Entity<Comentarios>().ToTable("Comentarios");
            modelBuilder.Entity<Usuarios>().ToTable("Usuarios");
            modelBuilder.Entity<Likes>().ToTable("Likes");

            // Configuração dos relacionamentos de Follow
            modelBuilder.Entity<Seguidores>()
                .HasOne(f => f.Usuario)
                .WithMany(u => u.Seguindo)
                .HasForeignKey(f => f.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Seguidores>()
                .HasOne(f => f.Seguido)
                .WithMany(u => u.Seguidores)
                .HasForeignKey(f => f.SeguidoId)
                .OnDelete(DeleteBehavior.Restrict);
            // Configuração dos relacionamentos de Like
            modelBuilder.Entity<Likes>()
                .HasOne(l => l.Usuarios)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UsuariosId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
