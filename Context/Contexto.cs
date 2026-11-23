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
}
