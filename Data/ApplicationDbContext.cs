using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PGDCP.Models;

namespace PGDCP.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // ── Tablas principales ──
        public DbSet<PerfilUsuario> PerfilesUsuario { get; set; }
        public DbSet<LoginSeguridad> LoginSeguridad { get; set; }
        public DbSet<Obra> Obras { get; set; }
        public DbSet<Conservacion> Conservaciones { get; set; }
        public DbSet<Valoracion> Valoraciones { get; set; }

        // ── Catálogo ──
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Epoca> Epocas { get; set; }
        public DbSet<Estilo> Estilos { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<EstadoConservacion> EstadosConservacion { get; set; }
        public DbSet<Tecnica> Tecnicas { get; set; }

        // ── Tablas relacionales y Auditoría ──
        public DbSet<ObraImagen> ObraImagenes { get; set; }
        public DbSet<ObraMaterial> ObraMateriales { get; set; }
        public DbSet<AuditoriaLogin> AuditoriasLogin { get; set; }
        public DbSet<LogCambio> LogCambios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LoginSeguridad>().ToTable("LoginSeguridad");
            builder.Entity<AuditoriaLogin>().ToTable("AuditoriaLogin");
            builder.Entity<LogCambio>().ToTable("LogCambios");

            builder.Entity<ObraMaterial>()
                .HasKey(om => new { om.ObraId, om.MaterialId });

            builder.Entity<LoginSeguridad>()
                .HasIndex(l => l.UserId)
                .IsUnique();

            builder.Entity<Obra>()
                .Property(o => o.ValorEstimado)
                .HasPrecision(18, 2);

            builder.Entity<Valoracion>()
                .Property(v => v.ValorEstimado)
                .HasPrecision(18, 2);

            builder.Entity<Conservacion>()
                .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany()
                .HasForeignKey(c => c.RestauradorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Valoracion>()
                .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany()
                .HasForeignKey(v => v.PeritoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Obra>()
                .HasOne(o => o.Tecnica)
                .WithMany(t => t.Obras)
                .HasForeignKey(o => o.TecnicaId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}