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

        public DbSet<Personagem> Personagens { get; set; }
        public DbSet<TipoPersonagem> TiposPersonagens { get; set; }
        //Sobrescrever o método OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Personagem>().ToTable("Personagens");
            modelBuilder.Entity<TipoPersonagem>().ToTable("TiposPersonagens");
        }
    }
}
