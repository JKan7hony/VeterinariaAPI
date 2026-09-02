using VeterinariaAPI.Models;
using VeterinariaAPI.Repositories;

namespace VeterinariaAPI.Endpoints
{
    public static class EspecialidadesApi
    {
        public static void MapEspecialidadApi(this WebApplication app)
        {
            var especialidad = app.MapGroup("/api/especialidades").WithTags("Especialidad");

            // API para listar especialidades
            especialidad.MapGet("/", async (IRepository<Especialidade> repo) =>
            {
                var listaEspecialidades = await repo.ObtenerTodosAsync();
                return Results.Ok(listaEspecialidades);
            });

            // API para crear una especialidad
            especialidad.MapPost("/", async (Especialidade e, IRepository<Especialidade> repo) =>
            {
                await repo.CrearAsync(e);
                await repo.GuardarCambiosAsync();
                return Results.Created($"/api/especialidades/{e.Id}", e);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));

            // API para editar especialidad por ID
            especialidad.MapPut("/{id:int}", async (int id, Especialidade e, IRepository<Especialidade> repo) =>
            {
                var especialidadExistente = await repo.ObtenerPorIdAsync(id);
                if (especialidadExistente is null) return Results.NotFound();

                especialidadExistente.Nombre = e.Nombre;
                especialidadExistente.CostoBase = e.CostoBase;

                await repo.GuardarCambiosAsync();
                return Results.Ok(especialidadExistente);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));

            // API para eliminar especialidades
            especialidad.MapDelete("/{id:int}", async (int id, IRepository<Especialidade> repo) =>
            {
                var especialidadExistente = await repo.ObtenerPorIdAsync(id);
                if (especialidadExistente is null) return Results.NotFound();

                await repo.EliminarAsync(especialidadExistente);
                await repo.GuardarCambiosAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));
        }
    }
}