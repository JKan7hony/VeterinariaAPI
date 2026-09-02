//using VeterinariaAPI.Models;
//using Microsoft.EntityFrameworkCore;

//namespace VeterinariaAPI.Endpoints
//{
//    public static class UsuariosApi
//    {
//        public static void MapUsuariosApi(this WebApplication app)
//        {
//            var usuarios = app.MapGroup("/api/usuarios").WithTags("Usuarios");

//            //Api para listar usuarios
//            usuarios.MapGet("/", async (VeterinariodbContext db) =>
//            {
//                var listaUsuarios = await db.Usuarios.ToListAsync();
//                return Results.Ok(listaUsuarios);
//            });

//            //API para crear un usuario
//            usuarios.MapPost("/", async (Usuario u, VeterinariodbContext db) =>
//            {
//                db.Usuarios.Add(u);
//                await db.SaveChangesAsync();
//                return Results.Created($"/api/usuarios/{u.Id}", u);
//            });

//            //API para editar usuario por ID
//            usuarios.MapPut("/{id:int}", async (int id, Usuario u, VeterinariodbContext db) =>
//            {
//                var usuarios = await db.Usuarios.FindAsync(id);
//                if (usuarios is null) return Results.NotFound();

//                usuarios.RolId = u.RolId;
//                usuarios.NombreCompleto = u.NombreCompleto;
//                usuarios.Email = u.Email;
//                usuarios.PasswordHash = u.PasswordHash;

//                await db.SaveChangesAsync();
//                return Results.Ok(usuarios);
//            });

//            //API para eliminar usuarios
//            usuarios.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
//            {
//                var usuarios = await db.Usuarios.FindAsync(id);
//                if (usuarios is null) return Results.NotFound();

//                db.Usuarios.Remove(usuarios);
//                await db.SaveChangesAsync();
//                return Results.NoContent();
//            });
//        }
//    }
//}
using VeterinariaAPI.Models;
using VeterinariaAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VeterinariaAPI.Endpoints
{
    public static class UsuariosApi
    {
        public static void MapUsuariosApi(this WebApplication app)
        {
            var usuarios = app.MapGroup("/api/usuarios").WithTags("Usuarios");

            // API para listar usuarios (incluyendo el nombre de su rol)
            usuarios.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaUsuarios = await db.Usuarios
                    .Include(u => u.Rol)
                    .Select(u => new {
                        u.Id,
                        u.NombreCompleto,
                        u.Email,
                        Rol = u.Rol != null ? u.Rol.Nombre : "Sin Rol"
                    })
                    .ToListAsync();

                return Results.Ok(listaUsuarios);
            });

            // API para registrar/crear un usuario con contraseña hasheada
            usuarios.MapPost("/register", async (UsuarioRegisterDto dto, VeterinariodbContext db, AuthService auth) =>
            {
                var nuevoUsuario = new Usuario
                {
                    RolId = dto.RolId,
                    NombreCompleto = dto.NombreCompleto,
                    Email = dto.Email
                };

                nuevoUsuario.PasswordHash = auth.HashPassword(nuevoUsuario, dto.Password);

                db.Usuarios.Add(nuevoUsuario);
                await db.SaveChangesAsync();

                return Results.Created($"/api/usuarios/{nuevoUsuario.Id}", new
                {
                    nuevoUsuario.Id,
                    nuevoUsuario.NombreCompleto,
                    nuevoUsuario.Email,
                    nuevoUsuario.RolId
                });
            });

            // API para Iniciar Sesión (Login) y obtener JWT Token
            usuarios.MapPost("/login", async (LoginRequest login, VeterinariodbContext db, AuthService auth, IConfiguration config) =>
            {
                // Obtenemos el usuario e INCLUIMOS la tabla Roles asociada por FK
                var usuario = await db.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Email == login.Email);

                if (usuario is null || !auth.VerifyPassword(usuario, login.Password))
                    return Results.Unauthorized();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    // Asignamos el nombre del rol traído desde la tabla Roles
                    new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "Cliente")
                };

                var jwtKey = config["Jwt:Key"]!;
                var jwtIssuer = config["Jwt:Issuer"]!;
                var jwtAudience = config["Jwt:Audience"]!;
                var jwtExpireMinutes = config["Jwt:ExpireMinutes"]!;

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtExpireMinutes)),
                    signingCredentials: credenciales
                );

                return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            });

            // API para eliminar usuario por ID
            usuarios.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var usuario = await db.Usuarios.FindAsync(id);
                if (usuario is null) return Results.NotFound();

                db.Usuarios.Remove(usuario);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }

        // DTOs para estructurar las peticiones de entrada
        public record UsuarioRegisterDto(int RolId, string NombreCompleto, string Email, string Password);
        public record LoginRequest(string Email, string Password);
    }
}