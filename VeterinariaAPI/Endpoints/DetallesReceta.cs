using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class DetallesRecetaApi
    {
        public static void MapDetalleRecetaApi(this WebApplication app)
        {
            var detallesRe = app.MapGroup("/api/detallesRe").WithTags("DetalleReceta");

            //Api para listar detalles de receta
            detallesRe.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaDetallesRe = await db.DetallesReceta.ToListAsync();
                return Results.Ok(listaDetallesRe);
            });

            //API para crear un detalle de receta
            detallesRe.MapPost("/", async (DetallesRecetum dr, VeterinariodbContext db) =>
            {
                db.DetallesReceta.Add(dr);
                await db.SaveChangesAsync();
                return Results.Created($"/api/detallesRe/{dr.Id}", dr);
            });

            //API para editar detalle de receta por ID
            detallesRe.MapPut("/{id:int}", async (int id, DetallesRecetum dr, VeterinariodbContext db) =>
            {
                var detallesRe = await db.DetallesReceta.FindAsync(id);
                if (detallesRe is null) return Results.NotFound();

                detallesRe.RecetaId = dr.RecetaId;
                detallesRe.InsumoId = dr.InsumoId;
                detallesRe.Dosis = dr.Dosis;
                detallesRe.DuracionDias = dr.DuracionDias;

                await db.SaveChangesAsync();
                return Results.Ok(detallesRe);
            });

            //API para eliminar detalles de receta
            detallesRe.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var detallesRe = await db.DetallesReceta.FindAsync(id);
                if (detallesRe is null) return Results.NotFound();

                db.DetallesReceta.Remove(detallesRe);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
