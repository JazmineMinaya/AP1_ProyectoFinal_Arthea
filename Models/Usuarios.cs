using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AP1_ProyectoFinal_Arthea.Models;

public class Usuarios : IdentityUser
{
    [Required(ErrorMessage = "Ingrese su nombre")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese su apellido")]
    public string Apellidos { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public bool Estado { get; set; } = true;
}
