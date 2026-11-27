using AP1_ProyectoFinal_Arthea.Components;
using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using AP1_ProyectoFinal_Arthea.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var ConStr = builder.Configuration.GetConnectionString("SqlConStr");

builder.Services.AddDbContextFactory<Contexto>(o => o.UseSqlServer(ConStr));
builder.Services.AddScoped<Contexto>(sp => sp.GetRequiredService<IDbContextFactory<Contexto>>().CreateDbContext());

builder.Services.AddScoped<AdministradoresService>();
builder.Services.AddScoped<ArtistasService>();
builder.Services.AddScoped<ServiciosService>();
builder.Services.AddScoped<ClientesService>();
builder.Services.AddScoped<ObrasService>();
builder.Services.AddScoped<PedidosService>();
builder.Services.AddScoped<PortafoliosService>();
builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<RegistroService>();

builder.Services.AddIdentity<Usuarios, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<Contexto>()
.AddDefaultTokenProviders();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddBlazorBootstrap();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/auth/login", async (HttpContext http, UserManager<Usuarios> userManager, SignInManager<Usuarios> signInManager) =>
{
    var formulario = await http.Request.ReadFormAsync();
    var correo = formulario["Correo"].ToString().Trim();
    var password = formulario["Password"].ToString();

    if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(password))
        return Results.LocalRedirect("/login?error=campos");

    var usuario = await userManager.FindByEmailAsync(correo);

    if (usuario == null || !usuario.Estado)
        return Results.LocalRedirect("/login?error=credenciales");

    var resultado = await signInManager.PasswordSignInAsync(usuario, password, isPersistent: false, lockoutOnFailure: true);

    if (!resultado.Succeeded)
        return Results.LocalRedirect(resultado.IsLockedOut ? "/login?error=bloqueado" : "/login?error=credenciales");

    return Results.LocalRedirect("/inicio");
});

app.Run();
