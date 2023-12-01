using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProyectoPI.Models
{
    public partial class DBPRUEBAContext : DbContext
    {
        public DBPRUEBAContext()
        {
        }

        public DBPRUEBAContext(DbContextOptions<DBPRUEBAContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Maki> Makis { get; set; } = null!;
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Consultas> Consultas { get; set; }
        public DbSet<Mensajes> Mensajes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Maki>(entity =>
            {
                entity.Property(e => e.Precio)
                    .HasColumnType("decimal(18, 2)");
            });
            modelBuilder.Entity<DetalleVenta>()
            .Property(d => d.Id)
            .ValueGeneratedNever();

            modelBuilder.Entity<Venta>()
            .Property(d => d.Id)
            .ValueGeneratedNever();

            modelBuilder.Entity<Venta>(entity =>
            {
                entity.Property(e => e.MontoTotal)
                    .HasColumnType("decimal(18, 2)");
            });
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario)
                    .HasName("PK__USUARIO__5B65BF97CFA2A5B0");

                entity.ToTable("USUARIO");

                entity.Property(e => e.Clave)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Correo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
