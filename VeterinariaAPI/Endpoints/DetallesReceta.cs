using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;

namespace VeterinariaAPI.Endpoints
{
    public static class DetallesRecetaApi
    {
        public static void MapDetalleRecetaApi(this WebApplication app)
        {
            var detallesRe = app.MapGroup("/api/detallesRe").WithTags("DetalleReceta");

            // API para listar detalles de receta
            detallesRe.MapGet("/", async (IRepository<DetallesRecetum> repo) =>
            {
                var listaDetallesRe = await repo.ObtenerTodosAsync();
                return Results.Ok(listaDetallesRe);
            });

            // API para crear un detalle de receta
            detallesRe.MapPost("/", async (DetallesRecetum dr, IRepository<DetallesRecetum> repo) =>
            {
                await repo.CrearAsync(dr);
                await repo.GuardarCambiosAsync();
                return Results.Created($"/api/detallesRe/{dr.Id}", dr);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para editar detalle de receta por ID
            detallesRe.MapPut("/{id:int}", async (int id, DetallesRecetum dr, IRepository<DetallesRecetum> repo) =>
            {
                var detalleReExistente = await repo.ObtenerPorIdAsync(id);
                if (detalleReExistente is null) return Results.NotFound();

                detalleReExistente.RecetaId = dr.RecetaId;
                detalleReExistente.InsumoId = dr.InsumoId;
                detalleReExistente.Dosis = dr.Dosis;
                detalleReExistente.DuracionDias = dr.DuracionDias;

                await repo.GuardarCambiosAsync();
                return Results.Ok(detalleReExistente);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para eliminar detalles de receta
            detallesRe.MapDelete("/{id:int}", async (int id, IRepository<DetallesRecetum> repo) =>
            {
                var detalleReExistente = await repo.ObtenerPorIdAsync(id);
                if (detalleReExistente is null) return Results.NotFound();

                await repo.EliminarAsync(detalleReExistente);
                await repo.GuardarCambiosAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}