using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VeterinariaAPI.Endpoints;
using VeterinariaAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VeterinariaAPI.Services;
using VeterinariaAPI.Repositories;




var builder = WebApplication.CreateBuilder(args);
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

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddScoped<AuthService>();

builder.Services.AddDbContext<VeterinariodbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("miconexion")));

// =========================================================
// 2. CONSTRUIR LA APLICACIÓN
// =========================================================
var app = builder.Build();

//app.UseAuthentication();

// =========================================================
// 3. CONFIGURACIÓN DEL PIPELINE HTTP
// =========================================================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Disponible en la ruta /scalar/v1
}

app.UseAuthentication();
app.UseAuthorization();

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

//Revisar Usuario API
app.MapUsuariosApi();
app.Run();