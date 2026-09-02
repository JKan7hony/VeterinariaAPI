using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VeterinariaAPI.Endpoints;
using VeterinariaAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// 1. REGISTRO DE SERVICIOS (Siempre ANTES de builder.Build)
// =========================================================
builder.Services.AddOpenApi();

builder.Services.AddDbContext<VeterinariodbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("miconexion")));

// =========================================================
// 2. CONSTRUIR LA APLICACIÓN
// =========================================================
var app = builder.Build();

// =========================================================
// 3. CONFIGURACIÓN DEL PIPELINE HTTP
// =========================================================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Disponible en la ruta /scalar/v1
}

app.MapRolApi();
app.MapEspecialidadApi();
app.MapClienteApi();
app.MapFacturasApi();
app.MapPacientesApi();
app.MapInsumosApi();
app.MapDetalleRecetaApi();
app.MapCitasApi();

//Revisar Usuario API
app.MapUsuariosApi();
app.Run();