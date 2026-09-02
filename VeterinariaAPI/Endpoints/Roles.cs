using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;

namespace VeterinariaAPI.Endpoints
{
    public static class RolesApi
    {
        public static void MapRolApi(this WebApplication app)
        {
            var roles = app.MapGroup("/api/roles").WithTags("Roles");

            // API para listar roles
            roles.MapGet("/", async (IRepository<Role> repo) =>
            {
                var listaRoles = await repo.ObtenerTodosAsync();
                return Results.Ok(listaRoles);
            });

            // API para crear un rol
            roles.MapPost("/", async (Role r, IRepository<Role> repo) =>
            {
                await repo.CrearAsync(r);
                await repo.GuardarCambiosAsync();
                return Results.Created($"/api/roles/{r.Id}", r);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para editar rol por ID
            roles.MapPut("/{id:int}", async (int id, Role r, IRepository<Role> repo) =>
            {
                var rolExistente = await repo.ObtenerPorIdAsync(id);
                if (rolExistente is null) return Results.NotFound();

                rolExistente.Nombre = r.Nombre;
                rolExistente.PermisosEscritura = r.PermisosEscritura;

                await repo.GuardarCambiosAsync();
                return Results.Ok(rolExistente);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para eliminar roles
            roles.MapDelete("/{id:int}", async (int id, IRepository<Role> repo) =>
            {
                var rolExistente = await repo.ObtenerPorIdAsync(id);
                if (rolExistente is null) return Results.NotFound();

                await repo.EliminarAsync(rolExistente);
                await repo.GuardarCambiosAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}