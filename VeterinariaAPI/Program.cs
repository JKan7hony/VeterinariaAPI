using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;
using VeterinariaAPI.Endpoints;
using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;
using VeterinariaAPI.Services;
using VeterinariaAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Configuración de JSON para evitar errores por ciclos de navegación en EF Core
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Configuración de JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Inyección de Dependencias (Repositorios y Servicios)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<VeterinariodbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("miconexion")));

// =========================================================
// CONSTRUIR LA APLICACIÓN
// =========================================================
var app = builder.Build();

// =========================================================
// CONFIGURACIÓN DEL PIPELINE HTTP
// =========================================================
app.UseMiddleware<GlobalExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

// Mapeo de Endpoints
app.MapRolApi();
app.MapEspecialidadApi();
app.MapClienteApi();
app.MapFacturasApi();
app.MapPacientesApi();
app.MapInsumosApi();
app.MapDetalleRecetaApi();
app.MapCitasApi();
app.MapRecetaApi();
app.MapDetalleFacApi();
app.MapConsultasApi();
app.MapUsuariosApi();
app.MapGet("/api/test-error", () =>
{
    throw new Exception("Prueba de falla capturada por el middleware");
});

app.Run();