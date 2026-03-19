using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PGDCP.Models;

namespace PGDCP.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<PerfilUsuario> PerfilesUsuario { get; set; }

        public DbSet<Obra> Obras { get; set; }
        public DbSet<Conservacion> Conservaciones { get; set; }
        public DbSet<Valoracion> Valoraciones { get; set; }

        public DbSet<LoginUsuario> LoginUsuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Obra>().Property(o => o.ValorEstimado).HasColumnType("decimal(18,2)");
            builder.Entity<Valoracion>().Property(v => v.ValorEstimado).HasColumnType("decimal(18,2)");
        }
    }
}
