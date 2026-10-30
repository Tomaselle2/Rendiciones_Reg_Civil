using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RendicionesRegistroCivil.Models;

namespace RendicionesRegistroCivil.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ArchivosRendicione> ArchivosRendiciones { get; set; }

    public virtual DbSet<DetalleRendicion> DetalleRendicions { get; set; }

    public virtual DbSet<Rendicione> Rendiciones { get; set; }

    public virtual DbSet<Tramite> Tramites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArchivosRendicione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("archivos_rendiciones_pkey");

            entity.ToTable("archivos_rendiciones");

            entity.HasIndex(e => e.NombreAlmacenado, "uq_archivo_nombre_almacenado").IsUnique();

            entity.HasIndex(e => e.RendicionId, "uq_archivo_rendicion").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.FechaCarga)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_carga");
            entity.Property(e => e.NombreAlmacenado)
                .HasMaxLength(255)
                .HasColumnName("nombre_almacenado");
            entity.Property(e => e.NombreOriginal)
                .HasMaxLength(255)
                .HasColumnName("nombre_original");
            entity.Property(e => e.RendicionId).HasColumnName("rendicion_id");
            entity.Property(e => e.RutaRelativa)
                .HasMaxLength(500)
                .HasColumnName("ruta_relativa");
            entity.Property(e => e.TamanoBytes).HasColumnName("tamano_bytes");
            entity.Property(e => e.TipoMime)
                .HasMaxLength(150)
                .HasColumnName("tipo_mime");

            entity.HasOne(d => d.Rendicion).WithOne(p => p.ArchivosRendicione)
                .HasForeignKey<ArchivosRendicione>(d => d.RendicionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_archivo_rendicion");
        });

        modelBuilder.Entity<DetalleRendicion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("detalle_rendicion_pkey");

            entity.ToTable("detalle_rendicion");

            entity.HasIndex(e => e.TramiteId, "idx_detalle_tramite");

            entity.HasIndex(e => new { e.RendicionId, e.TramiteId }, "uq_detalle_rendicion_tramite").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(12, 2)
                .HasColumnName("precio_unitario");
            entity.Property(e => e.RendicionId).HasColumnName("rendicion_id");
            entity.Property(e => e.Subtotal)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("((cantidad)::numeric * precio_unitario)", true)
                .HasColumnName("subtotal");
            entity.Property(e => e.TramiteId).HasColumnName("tramite_id");

            entity.HasOne(d => d.Rendicion).WithMany(p => p.DetalleRendicions)
                .HasForeignKey(d => d.RendicionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_detalle_rendicion");

            entity.HasOne(d => d.Tramite).WithMany(p => p.DetalleRendicions)
                .HasForeignKey(d => d.TramiteId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_detalle_tramite");
        });

        modelBuilder.Entity<Rendicione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rendiciones_pkey");

            entity.ToTable("rendiciones");

            entity.HasIndex(e => e.FechaCreacion, "idx_rendiciones_fecha_creacion");

            entity.HasIndex(e => new { e.PeriodoInicio, e.PeriodoFin }, "uq_rendiciones_periodo").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.PeriodoFin).HasColumnName("periodo_fin");
            entity.Property(e => e.PeriodoInicio).HasColumnName("periodo_inicio");
        });

        modelBuilder.Entity<Tramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tramites_pkey");

            entity.ToTable("tramites");

            entity.HasIndex(e => e.Nombre, "uq_tramites_nombre").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasPrecision(12, 2)
                .HasColumnName("precio");
            entity.Property(e => e.TipoTramite)
                .HasMaxLength(30)
                .HasColumnName("tipo_tramite");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
