using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;

namespace VeterinariaAPI.Endpoints
{
    public static class ClientesApi
    {
        public static void MapClienteApi(this WebApplication app)
        {
            var clientes = app.MapGroup("/api/clientes").WithTags("Clientes");

            // API para listar clientes
            clientes.MapGet("/", async (IRepository<Cliente> repo) =>
            {
                var listaClientes = await repo.ObtenerTodosAsync();
                return Results.Ok(listaClientes);
            });

            // API para crear un cliente
            clientes.MapPost("/", async (Cliente c, IRepository<Cliente> repo) =>
            {
                await repo.CrearAsync(c);
                await repo.GuardarCambiosAsync();
                return Results.Created($"/api/clientes/{c.Id}", c);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para editar un cliente por ID
            clientes.MapPut("/{id:int}", async (int id, Cliente c, IRepository<Cliente> repo) =>
            {
                var clienteExistente = await repo.ObtenerPorIdAsync(id);
                if (clienteExistente is null) return Results.NotFound();

                clienteExistente.DocumentoIdentidad = c.DocumentoIdentidad;
                clienteExistente.NombreCompleto = c.NombreCompleto;
                clienteExistente.Telefono = c.Telefono;
                clienteExistente.Email = c.Email;

                await repo.GuardarCambiosAsync();
                return Results.Ok(clienteExistente);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));

            // API para eliminar cliente
            clientes.MapDelete("/{id:int}", async (int id, IRepository<Cliente> repo) =>
            {
                var clienteExistente = await repo.ObtenerPorIdAsync(id);
                if (clienteExistente is null) return Results.NotFound();

                await repo.EliminarAsync(clienteExistente);
                await repo.GuardarCambiosAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));
        }
    }
}