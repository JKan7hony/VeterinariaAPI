using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;

namespace VeterinariaAPI.Endpoints
{
    public static class RecetasApi
    {
        public static void MapRecetaApi(this WebApplication app)
        {
            var recetas = app.MapGroup("/api/v1/recetas").WithTags("Recetas(v1)");

            // API para listar recetas
            recetas.MapGet("/", async (IRepository<Receta> repo) =>
            {
                var listaRecetas = await repo.ObtenerTodosAsync();
                return Results.Ok(listaRecetas);
            });

            // API para crear una receta
            recetas.MapPost("/", async (Receta r, IRepository<Receta> repo) =>
            {
                await repo.CrearAsync(r);
                await repo.GuardarCambiosAsync();
                return Results.Created($"/api/recetas/{r.Id}", r);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para editar una receta por ID
            recetas.MapPut("/{id:int}", async (int id, Receta r, IRepository<Receta> repo) =>
            {
                var recetaExistente = await repo.ObtenerPorIdAsync(id);
                if (recetaExistente is null) return Results.NotFound("La receta no existe");

                recetaExistente.ConsultaId = r.ConsultaId;
                recetaExistente.FechaEmision = r.FechaEmision;
                recetaExistente.ValidaHasta = r.ValidaHasta;

                await repo.GuardarCambiosAsync();
                return Results.Ok(recetaExistente);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para eliminar recetas
            recetas.MapDelete("/{id:int}", async (int id, IRepository<Receta> repo) =>
            {
                var recetaExistente = await repo.ObtenerPorIdAsync(id);
                if (recetaExistente is null) return Results.NotFound();

                await repo.EliminarAsync(recetaExistente);
                await repo.GuardarCambiosAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}