using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class RecetasApi
    {
        public static void MapRecetaApi(this WebApplication app)
        {
            var recetas = app.MapGroup("/api/recetas").WithTags("Recetas");

            //Api para listar recetas
            recetas.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaRecetas = await db.Recetas.ToListAsync();
                return Results.Ok(listaRecetas);
            });

            //API para crear una receta
            recetas.MapPost("/", async (Receta r, VeterinariodbContext db) =>
            {
                db.Recetas.Add(r);
                await db.SaveChangesAsync();
                return Results.Created($"/api/recetas/{r.Id}", r);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            //API para editar una receta por ID
            recetas.MapPut("/{id:int}", async (int id, Receta r, VeterinariodbContext db) =>
            {
                var recetas = await db.Recetas.FindAsync(id);
                if (recetas is null) return Results.NotFound("La receta no existe");

                recetas.ConsultaId = r.ConsultaId;
                recetas.FechaEmision = r.FechaEmision;
                recetas.ValidaHasta = r.ValidaHasta;

                await db.SaveChangesAsync();
                return Results.Ok(recetas);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            //API para eliminar recetas
            recetas.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var recetas = await db.Recetas.FindAsync(id);
                if (recetas is null) return Results.NotFound();

                db.Recetas.Remove(recetas);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}
