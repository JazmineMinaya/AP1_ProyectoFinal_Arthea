using Microsoft.EntityFrameworkCore;

namespace AP1_ProyectoFinal_Arthea.Context;


public class Contexto : DbContext
{
    public Contexto(DbContextOptions<Contexto> options) : base(options) { }

}
