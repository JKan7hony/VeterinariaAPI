using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;

namespace VeterinariaAPI.Endpoints
{
    public static class PacientesApi
    {
        public static void MapPacientesApi(this WebApplication app)
        {
            var pacientes = app.MapGroup("/api/v1/pacientes").WithTags("pacientes(v1)");

            // API para listar pacientes
            pacientes.MapGet("/", async (IRepository<Paciente> repo) =>
            {
                var listaPacientes = await repo.ObtenerTodosAsync();
                return Results.Ok(listaPacientes);
            });

            // API para crear un paciente
            pacientes.MapPost("/", async (Paciente p, IRepository<Paciente> repo) =>
            {
                await repo.CrearAsync(p);
                await repo.GuardarCambiosAsync();
                return Results.Created($"/api/pacientes/{p.Id}", p);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para editar paciente por ID
            pacientes.MapPut("/{id:int}", async (int id, Paciente p, IRepository<Paciente> repo) =>
            {
                var pacienteExistente = await repo.ObtenerPorIdAsync(id);
                if (pacienteExistente is null) return Results.NotFound();

                pacienteExistente.ClienteId = p.ClienteId;
                pacienteExistente.Nombre = p.Nombre;
                pacienteExistente.Especie = p.Especie;
                pacienteExistente.Raza = p.Raza;
                pacienteExistente.Peso = p.Peso;
                pacienteExistente.Alergias = p.Alergias;

                await repo.GuardarCambiosAsync();
                return Results.Ok(pacienteExistente);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));

            // API para eliminar pacientes
            pacientes.MapDelete("/{id:int}", async (int id, IRepository<Paciente> repo) =>
            {
                var pacienteExistente = await repo.ObtenerPorIdAsync(id);
                if (pacienteExistente is null) return Results.NotFound();

                await repo.EliminarAsync(pacienteExistente);
                await repo.GuardarCambiosAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Veterinario"));
        }
    }
}