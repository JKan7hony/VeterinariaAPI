using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class ClientesApi
    {
        public static void MapClienteApi(this WebApplication app)
        {
            var clientes = app.MapGroup("/api/clientes").WithTags("Clientes");

            //Api para listar clientes
            clientes.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaClientes = await db.Clientes.ToListAsync();
                return Results.Ok(listaClientes);
            });

            //API para crear un cliente
            clientes.MapPost("/", async (Cliente c, VeterinariodbContext db) =>
            {
                db.Clientes.Add(c);
                await db.SaveChangesAsync();
                return Results.Created($"/api/clientes/{c.Id}", c);
            });

            //API para editar un cliente por ID
            clientes.MapPut("/{id:int}", async (int id, Cliente c, VeterinariodbContext db) =>
            {
                var clientes = await db.Clientes.FindAsync(id);
                if (clientes is null) return Results.NotFound();

                clientes.DocumentoIdentidad = c.DocumentoIdentidad;
                clientes.NombreCompleto = c.NombreCompleto;
                clientes.Telefono = c.Telefono;
                clientes.Email = c.Email;

                await db.SaveChangesAsync();
                return Results.Ok(clientes);
            });

            //API para eliminar cliente
            clientes.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var clientes = await db.Clientes.FindAsync(id);
                if (clientes is null) return Results.NotFound();

                db.Clientes.Remove(clientes);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
