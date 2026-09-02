using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class UsuariosApi
    {
        public static void MapUsuariosApi(this WebApplication app)
        {
            var usuarios = app.MapGroup("/api/usuarios").WithTags("Usuarios");

            //Api para listar usuarios
            usuarios.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaUsuarios = await db.Usuarios.ToListAsync();
                return Results.Ok(listaUsuarios);
            });

            //API para crear un usuario
            usuarios.MapPost("/", async (Usuario u, VeterinariodbContext db) =>
            {
                db.Usuarios.Add(u);
                await db.SaveChangesAsync();
                return Results.Created($"/api/usuarios/{u.Id}", u);
            });

            //API para editar usuario por ID
            usuarios.MapPut("/{id:int}", async (int id, Usuario u, VeterinariodbContext db) =>
            {
                var usuarios = await db.Usuarios.FindAsync(id);
                if (usuarios is null) return Results.NotFound();

                usuarios.RolId = u.RolId;
                usuarios.NombreCompleto = u.NombreCompleto;
                usuarios.Email = u.Email;
                usuarios.PasswordHash = u.PasswordHash;
                    
                await db.SaveChangesAsync();
                return Results.Ok(usuarios);
            });

            //API para eliminar usuarios
            usuarios.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var usuarios = await db.Usuarios.FindAsync(id);
                if (usuarios is null) return Results.NotFound();

                db.Usuarios.Remove(usuarios);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
