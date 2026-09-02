using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class FacturasApi
    {
        public static void MapFacturasApi(this WebApplication app)
        {
            var facturas = app.MapGroup("/api/facturas").WithTags("Facturas");

            //Api para listar facturas
            facturas.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaFacturas = await db.Facturas.ToListAsync();
                return Results.Ok(listaFacturas);
            });

            //API para crear una factura
            facturas.MapPost("/", async (Factura f, VeterinariodbContext db) =>
            {
                db.Facturas.Add(f);
                await db.SaveChangesAsync();
                return Results.Created($"/api/facturas/{f.Id}", f);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            //API para editar factura por ID
            facturas.MapPut("/{id:int}", async (int id, Factura f, VeterinariodbContext db) =>
            {
                var facturas = await db.Facturas.FindAsync(id);
                if (facturas is null) return Results.NotFound();

                facturas.ClienteId = f.ClienteId;
                facturas.FechaEmision = f.FechaEmision;
                facturas.MontoImpuestos = f.MontoImpuestos;
                facturas.MontoTotal = f.MontoTotal;

                await db.SaveChangesAsync();
                return Results.Ok(facturas);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            //API para eliminar Facturas
            facturas.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var facturas = await db.Facturas.FindAsync(id);
                if (facturas is null) return Results.NotFound();

                db.Facturas.Remove(facturas);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}
