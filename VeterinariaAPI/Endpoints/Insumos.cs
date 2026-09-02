using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class InsumosApi
    {
        public static void MapInsumosApi(this WebApplication app)
        {
            var insumos = app.MapGroup("/api/insumos").WithTags("Insumos");

            //Api para listar insumos
            insumos.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaInsumos = await db.Insumos.ToListAsync();
                return Results.Ok(listaInsumos);
            });

            //API para crear un insumo
            insumos.MapPost("/", async (Insumo i, VeterinariodbContext db) =>
            {
                db.Insumos.Add(i);
                await db.SaveChangesAsync();
                return Results.Created($"/api/insumos/{i.Id}", i);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            //API para editar insumo por ID
            insumos.MapPut("/{id:int}", async (int id, Insumo i, VeterinariodbContext db) =>
            {
                var insumos = await db.Insumos.FindAsync(id);
                if (insumos is null) return Results.NotFound();

                insumos.NombreProducto = i.NombreProducto;
                insumos.Tipo = i.Tipo;
                insumos.StockActual = i.StockActual;
                insumos.PrecioUnitario = i.PrecioUnitario;

                await db.SaveChangesAsync();
                return Results.Ok(insumos);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            //API para eliminar insumos
            insumos.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var insumos = await db.Insumos.FindAsync(id);
                if (insumos is null) return Results.NotFound();

                db.Insumos.Remove(insumos);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}
