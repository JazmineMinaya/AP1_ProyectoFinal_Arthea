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
builder.Services.AddScoped<AdministradoresService>();
builder.Services.AddScoped<ArtistasService>();
builder.Services.AddScoped<ClientesService>();
builder.Services.AddScoped<ObrasService>();

builder.Services.AddIdentity<Usuarios, IdentityRole>()
    .AddEntityFrameworkStores<Contexto>()
    .AddDefaultTokenProviders();

builder.Services.AddBlazorBootstrap();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

app.Run();
