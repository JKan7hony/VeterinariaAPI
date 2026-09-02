using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;

namespace VeterinariaAPI.Endpoints
{
    public static class CitasApi
    {
        public static void MapCitasApi(this WebApplication app)
        {
            var citas = app.MapGroup("/api/citas").WithTags("Citas");

            // Api para listar citas
            citas.MapGet("/", async (IRepository<Cita> repo) =>
            {
                var listarCitas = await repo.ObtenerTodosAsync();
                return Results.Ok(listarCitas);
            });

            // API para crear una cita
            citas.MapPost("/", async (Cita c, IRepository<Cita> repo) =>
            {
                await repo.CrearAsync(c);
                await repo.GuardarCambiosAsync();
                return Results.Created($"/api/citas/{c.Id}", c);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para editar una cita por ID
            citas.MapPut("/{id:int}", async (int id, Cita c, IRepository<Cita> repo) =>
            {
                var citaExistente = await repo.ObtenerPorIdAsync(id);
                if (citaExistente is null) return Results.NotFound();

                citaExistente.PacienteId = c.PacienteId;
                citaExistente.UsuarioId = c.UsuarioId;
                citaExistente.EspecialidadId = c.EspecialidadId;
                citaExistente.FechaHora = c.FechaHora;
                citaExistente.Estado = c.Estado;

                await repo.GuardarCambiosAsync();
                return Results.Ok(citaExistente);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para eliminar citas
            citas.MapDelete("/{id:int}", async (int id, IRepository<Cita> repo) =>
            {
                var citaExistente = await repo.ObtenerPorIdAsync(id);
                if (citaExistente is null) return Results.NotFound();

                await repo.EliminarAsync(citaExistente);
                await repo.GuardarCambiosAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}