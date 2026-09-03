using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;

namespace VeterinariaAPI.Endpoints
{
    public static class FacturasApi
    {
        public static void MapFacturasApi(this WebApplication app)
        {
            var facturas = app.MapGroup("/api/v1/facturas").WithTags("Facturas (v1)");

            // API para listar facturas
            facturas.MapGet("/", async (IRepository<Factura> repo) =>
            {
                var listaFacturas = await repo.ObtenerTodosAsync();
                return Results.Ok(listaFacturas);
            });

            // API para crear una factura
            facturas.MapPost("/", async (Factura f, IRepository<Factura> repo) =>
            {
                await repo.CrearAsync(f);
                await repo.GuardarCambiosAsync();
                return Results.Created($"/api/facturas/{f.Id}", f);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para editar factura por ID
            facturas.MapPut("/{id:int}", async (int id, Factura f, IRepository<Factura> repo) =>
            {
                var facturaExistente = await repo.ObtenerPorIdAsync(id);
                if (facturaExistente is null) return Results.NotFound();

                facturaExistente.ClienteId = f.ClienteId;
                facturaExistente.FechaEmision = f.FechaEmision;
                facturaExistente.MontoImpuestos = f.MontoImpuestos;
                facturaExistente.MontoTotal = f.MontoTotal;

                await repo.GuardarCambiosAsync();
                return Results.Ok(facturaExistente);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para eliminar Facturas
            facturas.MapDelete("/{id:int}", async (int id, IRepository<Factura> repo) =>
            {
                var facturaExistente = await repo.ObtenerPorIdAsync(id);
                if (facturaExistente is null) return Results.NotFound();

                await repo.EliminarAsync(facturaExistente);
                await repo.GuardarCambiosAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}