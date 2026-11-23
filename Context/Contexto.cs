using AP1_ProyectoFinal_Arthea.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AP1_ProyectoFinal_Arthea.Context;

public class Contexto : IdentityDbContext<Usuarios>
{
    public Contexto(DbContextOptions<Contexto> options) : base(options) { }

    public DbSet<Administradores> Administradores { get; set; }
    public DbSet<Artistas> Artistas { get; set; }
    public DbSet<Servicios> Servicios { get; set; }
    public DbSet<Clientes> Clientes { get; set; }
    public DbSet<Obras> Obras { get; set; }
    public DbSet<Pedidos> Pedidos { get; set; }
    public DbSet<PedidosDetalle> PedidosDetalles { get; set; }
    public DbSet<Facturas> Facturas { get; set; }

    public DbSet<Portafolios> Portafolios { get; set; }
    public DbSet<PortafoliosDetalle> PortafoliosDetalle { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Pedidos>()
            .HasOne(p => p.Cliente)
            .WithMany()
            .HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Pedidos>()
            .HasOne(p => p.Artista)
            .WithMany()
            .HasForeignKey(p => p.ArtistaId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PortafoliosDetalle>()
        .HasOne(d => d.Obra)
        .WithMany()
        .HasForeignKey(d => d.ObraId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PortafoliosDetalle>()
            .HasOne(d => d.Portafolio)
            .WithMany(p => p.PortafoliosDetalle)
            .HasForeignKey(d => d.PortafolioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
