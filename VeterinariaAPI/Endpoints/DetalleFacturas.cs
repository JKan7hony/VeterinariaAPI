using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class DetalleFacturasAPI
    {
        public static void MapDetalleFacApi(this WebApplication app)
        {
            var detallefac = app.MapGroup("/api/detallefac").WithTags("DetalleFactura");

            //Api para listar detalle factura
            detallefac.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaDetalleFac = await db.DetallesFacturas.ToListAsync();
                return Results.Ok(listaDetalleFac);
            });

            //API para crear un detalle de factura
            detallefac.MapPost("/", async (DetallesFactura df, VeterinariodbContext db) =>
            {
                db.DetallesFacturas.Add(df);
                await db.SaveChangesAsync();
                return Results.Created($"/api/detallefac/{df.Id}", df);
            });

            //API para editar detalle factura por ID
            detallefac.MapPut("/{id:int}", async (int id, DetallesFactura df, VeterinariodbContext db) =>
            {
                var detallefac = await db.DetallesFacturas.FindAsync(id);
                if (detallefac is null) return Results.NotFound();

                detallefac.FacturaId = df.FacturaId;
                detallefac.ConsultaId = df.ConsultaId;
                detallefac.InsumoId = df.InsumoId;
                detallefac.Subtotal = df.Subtotal;

                await db.SaveChangesAsync();
                return Results.Ok(detallefac);
            });

            //API para eliminar un detalle de factura
            detallefac.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var detallefac = await db.DetallesFacturas.FindAsync(id);
                if (detallefac is null) return Results.NotFound();

                db.DetallesFacturas.Remove(detallefac);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
