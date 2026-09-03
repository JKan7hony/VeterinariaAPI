using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;
using VeterinariaAPI.Services;

namespace VeterinariaAPI.Endpoints
{
    public static class UsuariosApi
    {
        public static void MapUsuariosApi(this WebApplication app)
        {
            var usuarios = app.MapGroup("/api/v1/usuarios").WithTags("Usuarios(v1)");

            // API para listar usuarios (obtenidos desde el Repositorio)
            usuarios.MapGet("/", async (IUsuarioRepository repo) =>
            {
                var listaUsuarios = await repo.ObtenerTodosAsync();

                var respuesta = listaUsuarios.Select(u => new
                {
                    u.Id,
                    u.NombreCompleto,
                    u.Email,
                    Rol = u.Rol != null ? u.Rol.Nombre : "Sin Rol"
                });

                return Results.Ok(respuesta);
            });

            // API para registrar/crear un usuario
            usuarios.MapPost("/register", async (UsuarioRegisterDto dto, IUsuarioRepository repo, AuthService auth) =>
            {
                var nuevoUsuario = new Usuario
                {
                    RolId = dto.RolId,
                    NombreCompleto = dto.NombreCompleto,
                    Email = dto.Email
                };

                nuevoUsuario.PasswordHash = auth.HashPassword(nuevoUsuario, dto.Password);

                await repo.CrearAsync(nuevoUsuario);
                await repo.GuardarCambiosAsync();

                return Results.Created($"/api/usuarios/{nuevoUsuario.Id}", new
                {
                    nuevoUsuario.Id,
                    nuevoUsuario.NombreCompleto,
                    nuevoUsuario.Email,
                    nuevoUsuario.RolId
                });
            });

            // API para Iniciar Sesión (Login) y obtener JWT Token
            usuarios.MapPost("/login", async (LoginRequest login, IUsuarioRepository repo, AuthService auth, IConfiguration config) =>
            {
                // Obtenemos el usuario e incluimos su Rol mediante el Repositorio
                var usuario = await repo.ObtenerPorEmailAsync(login.Email);

                if (usuario is null || !auth.VerifyPassword(usuario, login.Password))
                    return Results.Unauthorized();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                    new Claim(ClaimTypes.Email, usuario.Email),
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

            // API para eliminar usuario por ID (Protegida por Roles)
            usuarios.MapDelete("/{id:int}", async (int id, IUsuarioRepository repo) =>
            {
                var usuario = await repo.ObtenerPorIdAsync(id);
                if (usuario is null) return Results.NotFound();

                await repo.EliminarAsync(usuario);
                await repo.GuardarCambiosAsync();

                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }

        // DTOs para estructurar las peticiones de entrada
        public record UsuarioRegisterDto(int RolId, string NombreCompleto, string Email, string Password);
        public record LoginRequest(string Email, string Password);
    }
}