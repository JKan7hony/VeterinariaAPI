using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class RolesApi
    {
        public static void MapRolApi(this WebApplication app)
        {
            var roles = app.MapGroup("/api/roles").WithTags("Roles");

            //Api para listar roles
            roles.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaRoles = await db.Roles.ToListAsync();
                return Results.Ok(listaRoles);
            });

            //API para crear un rol
            roles.MapPost("/", async (Role r, VeterinariodbContext db) =>
            {
                db.Roles.Add(r);
                await db.SaveChangesAsync();
                return Results.Created($"/api/roles/{r.Id}", r);
            });

            //API para editar rol por ID
            roles.MapPut("/{id:int}", async (int id, Role r, VeterinariodbContext db) =>
            {
                var roles = await db.Roles.FindAsync(id);
                if (roles is null) return Results.NotFound();

                roles.Nombre = r.Nombre;
                roles.PermisosEscritura = r.PermisosEscritura;

                await db.SaveChangesAsync();
                return Results.Ok(roles);
            });

            //API para eliminar roles
            roles.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var roles = await db.Roles.FindAsync(id);
                if (roles is null) return Results.NotFound();

                db.Roles.Remove(roles);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
