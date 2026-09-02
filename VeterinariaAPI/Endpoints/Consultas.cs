using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;

namespace VeterinariaAPI.Endpoints
{
    public static class ConsultasApi
    {
        public static void MapConsultasApi(this WebApplication app)
        {
            var consultas = app.MapGroup("/api/consultas").WithTags("Consultas");

            // API para listar consultas
            consultas.MapGet("/", async (IRepository<Consulta> repo) =>
            {
                var listaConsultas = await repo.ObtenerTodosAsync();
                return Results.Ok(listaConsultas);
            });

            // API para crear una consulta
            consultas.MapPost("/", async (Consulta c, IRepository<Consulta> repo) =>
            {
                await repo.CrearAsync(c);
                await repo.GuardarCambiosAsync();
                return Results.Created($"/api/consultas/{c.Id}", c);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para editar consulta por ID
            consultas.MapPut("/{id:int}", async (int id, Consulta c, IRepository<Consulta> repo) =>
            {
                var consultaExistente = await repo.ObtenerPorIdAsync(id);
                if (consultaExistente is null) return Results.NotFound();

                consultaExistente.PacienteId = c.PacienteId;
                consultaExistente.CitaId = c.CitaId;
                consultaExistente.Motivo = c.Motivo;
                consultaExistente.Diagnostico = c.Diagnostico;

                await repo.GuardarCambiosAsync();
                return Results.Ok(consultaExistente);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para eliminar consultas
            consultas.MapDelete("/{id:int}", async (int id, IRepository<Consulta> repo) =>
            {
                var consultaExistente = await repo.ObtenerPorIdAsync(id);
                if (consultaExistente is null) return Results.NotFound();

                await repo.EliminarAsync(consultaExistente);
                await repo.GuardarCambiosAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}