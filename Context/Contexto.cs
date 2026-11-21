using AP1_ProyectoFinal_Arthea.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AP1_ProyectoFinal_Arthea.Context;

public class Contexto : IdentityDbContext<Usuarios>
{
    public Contexto(DbContextOptions<Contexto> options) : base(options) { }

    public DbSet<Administradores> Administradores { get; set; }
    public DbSet<Artistas> Artistas { get; set; }
}
