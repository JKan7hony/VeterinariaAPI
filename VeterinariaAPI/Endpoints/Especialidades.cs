using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class EspecialidadesApi
    {
    public static void MapEspecialidadApi(this WebApplication app)
        {
            var especialidad = app.MapGroup("/api/especialidades").WithTags("Especialidad");

            //Api para listar especialidades
            especialidad.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaEspecialidades = await db.Especialidades.ToListAsync();
                return Results.Ok(listaEspecialidades);
            });

            //API para crear una especialiddad
            especialidad.MapPost("/", async (Especialidade e, VeterinariodbContext db) =>
            {
                db.Especialidades.Add(e);
                await db.SaveChangesAsync();
                return Results.Created($"/api/especialidades/{e.Id}", e);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));

            //API para editar especialidad por ID
            especialidad.MapPut("/{id:int}", async (int id, Especialidade e, VeterinariodbContext db) =>
            {
                var especialidades = await db.Especialidades.FindAsync(id);
                if (especialidades is null) return Results.NotFound();

                especialidades.Nombre = e.Nombre;
                especialidades.CostoBase = e.CostoBase;

                await db.SaveChangesAsync();
                return Results.Ok(especialidades);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));

            //API para eliminar especialidades
            especialidad.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var especialidades = await db.Especialidades.FindAsync(id);
                if (especialidades is null) return Results.NotFound();

                db.Especialidades.Remove(especialidades);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));
        }
    }
}
