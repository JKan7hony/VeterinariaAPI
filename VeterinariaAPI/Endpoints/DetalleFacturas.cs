using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;

namespace VeterinariaAPI.Endpoints
{
    public static class DetalleFacturasAPI
    {
        public static void MapDetalleFacApi(this WebApplication app)
        {
            var detallefac = app.MapGroup("/api/detallefac").WithTags("DetalleFactura");

            // API para listar detalle factura
            detallefac.MapGet("/", async (IRepository<DetallesFactura> repo) =>
            {
                var listaDetalleFac = await repo.ObtenerTodosAsync();
                return Results.Ok(listaDetalleFac);
            });

            // API para crear un detalle de factura
            detallefac.MapPost("/", async (DetallesFactura df, IRepository<DetallesFactura> repo) =>
            {
                await repo.CrearAsync(df);
                await repo.GuardarCambiosAsync();
                return Results.Created($"/api/detallefac/{df.Id}", df);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para editar detalle factura por ID
            detallefac.MapPut("/{id:int}", async (int id, DetallesFactura df, IRepository<DetallesFactura> repo) =>
            {
                var detallefacExistente = await repo.ObtenerPorIdAsync(id);
                if (detallefacExistente is null) return Results.NotFound();

                detallefacExistente.FacturaId = df.FacturaId;
                detallefacExistente.ConsultaId = df.ConsultaId;
                detallefacExistente.InsumoId = df.InsumoId;
                detallefacExistente.Subtotal = df.Subtotal;

                await repo.GuardarCambiosAsync();
                return Results.Ok(detallefacExistente);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para eliminar un detalle de factura
            detallefac.MapDelete("/{id:int}", async (int id, IRepository<DetallesFactura> repo) =>
            {
                var detallefacExistente = await repo.ObtenerPorIdAsync(id);
                if (detallefacExistente is null) return Results.NotFound();

                await repo.EliminarAsync(detallefacExistente);
                await repo.GuardarCambiosAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}