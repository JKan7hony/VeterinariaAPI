using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;

namespace VeterinariaAPI.Endpoints
{
    public static class InsumosApi
    {
        public static void MapInsumosApi(this WebApplication app)
        {
            var insumos = app.MapGroup("/api/insumos").WithTags("Insumos");

            // API para listar insumos
            insumos.MapGet("/", async (IRepository<Insumo> repo) =>
            {
                var listaInsumos = await repo.ObtenerTodosAsync();
                return Results.Ok(listaInsumos);
            });

            // API para crear un insumo
            insumos.MapPost("/", async (Insumo i, IRepository<Insumo> repo) =>
            {
                await repo.CrearAsync(i);
                await repo.GuardarCambiosAsync();
                return Results.Created($"/api/insumos/{i.Id}", i);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para editar insumo por ID
            insumos.MapPut("/{id:int}", async (int id, Insumo i, IRepository<Insumo> repo) =>
            {
                var insumoExistente = await repo.ObtenerPorIdAsync(id);
                if (insumoExistente is null) return Results.NotFound();

                insumoExistente.NombreProducto = i.NombreProducto;
                insumoExistente.Tipo = i.Tipo;
                insumoExistente.StockActual = i.StockActual;
                insumoExistente.PrecioUnitario = i.PrecioUnitario;

                await repo.GuardarCambiosAsync();
                return Results.Ok(insumoExistente);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para eliminar insumos
            insumos.MapDelete("/{id:int}", async (int id, IRepository<Insumo> repo) =>
            {
                var insumoExistente = await repo.ObtenerPorIdAsync(id);
                if (insumoExistente is null) return Results.NotFound();

                await repo.EliminarAsync(insumoExistente);
                await repo.GuardarCambiosAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}