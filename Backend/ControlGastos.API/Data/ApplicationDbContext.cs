using ControlGastos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlGastos.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoGasto> TiposGasto { get; set; }
        public DbSet<FondoMonetario> FondosMonetarios { get; set; }
        public DbSet<Presupuesto> Presupuestos { get; set; }
        public DbSet<RegistroGastoEncabezado> RegistrosGastoEncabezado { get; set; }
        public DbSet<RegistroGastoDetalle> RegistrosGastoDetalle { get; set; }
        public DbSet<Deposito> Depositos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar Rol
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasMany(r => r.Usuarios)
                    .WithOne(u => u.Rol)
                    .HasForeignKey(u => u.RolId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurar Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(e => e.NombreUsuario).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.RolId);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasMany(u => u.TipoGastos)
                    .WithOne(t => t.Usuario)
                    .HasForeignKey(t => t.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.FondosMonetarios)
                    .WithOne(f => f.Usuario)
                    .HasForeignKey(f => f.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Presupuestos)
                    .WithOne(p => p.Usuario)
                    .HasForeignKey(p => p.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.RegistrosGasto)
                    .WithOne(r => r.Usuario)
                    .HasForeignKey(r => r.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Depositos)
                    .WithOne(d => d.Usuario)
                    .HasForeignKey(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurar TipoGasto
            modelBuilder.Entity<TipoGasto>(entity =>
            {
                entity.HasIndex(e => new { e.UsuarioId, e.Codigo }).IsUnique();
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configurar FondoMonetario
            modelBuilder.Entity<FondoMonetario>(entity =>
            {
                entity.Property(e => e.SaldoActual).HasPrecision(18, 2);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configurar Presupuesto
            modelBuilder.Entity<Presupuesto>(entity =>
            {
                entity.Property(e => e.MontoPresupuestado).HasPrecision(18, 2);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => new { e.UsuarioId, e.TipoGastoId, e.Mes, e.Anio }).IsUnique();

                entity.HasOne(e => e.TipoGasto)
                    .WithMany(t => t.Presupuestos)
                    .HasForeignKey(e => e.TipoGastoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurar RegistroGastoEncabezado
            modelBuilder.Entity<RegistroGastoEncabezado>(entity =>
            {
                entity.Property(e => e.MontoTotal).HasPrecision(18, 2);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.FondoMonetario)
                    .WithMany(f => f.RegistrosGasto)
                    .HasForeignKey(e => e.FondoMonetarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // PostgreSQL no requiere UseSqlOutputClause
            });

            // Configurar RegistroGastoDetalle
            modelBuilder.Entity<RegistroGastoDetalle>(entity =>
            {
                entity.Property(e => e.Monto).HasPrecision(18, 2);

                entity.HasOne(e => e.RegistroGasto)
                    .WithMany(r => r.Detalles)
                    .HasForeignKey(e => e.RegistroGastoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.TipoGasto)
                    .WithMany(t => t.DetallesGasto)
                    .HasForeignKey(e => e.TipoGastoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurar Deposito
            modelBuilder.Entity<Deposito>(entity =>
            {
                entity.Property(e => e.Monto).HasPrecision(18, 2);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.FondoMonetario)
                    .WithMany(f => f.Depositos)
                    .HasForeignKey(e => e.FondoMonetarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // PostgreSQL no requiere UseSqlOutputClause
            });
        }
    }
}
