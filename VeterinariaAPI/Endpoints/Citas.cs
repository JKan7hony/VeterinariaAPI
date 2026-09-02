using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class CitasApi
    {
        public static void MapCitasApi(this WebApplication app)
        {
            var citas = app.MapGroup("/api/citas").WithTags("Citas");

            //Api para listar citas
            citas.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listarCitas = await db.Citas.ToListAsync();
                return Results.Ok(listarCitas);
            });

            //API para crear una cita
            citas.MapPost("/", async (Cita c, VeterinariodbContext db) =>
            {
                db.Citas.Add(c);
                await db.SaveChangesAsync();
                return Results.Created($"/api/citas/{c.Id}", c);
            });

            //API para editar rol por ID
            citas.MapPut("/{id:int}", async (int id, Cita c, VeterinariodbContext db) =>
            {
                var citas = await db.Citas.FindAsync(id);
                if (citas is null) return Results.NotFound();

                citas.PacienteId = c.PacienteId;
                citas.UsuarioId = c.UsuarioId;
                citas.EspecialidadId = c.EspecialidadId;
                citas.FechaHora = c.FechaHora;
                citas.Estado = c.Estado;

                await db.SaveChangesAsync();
                return Results.Ok(citas);
            });

            //API para eliminar citas
            citas.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var citas = await db.Citas.FindAsync(id);
                if (citas is null) return Results.NotFound();

                db.Citas.Remove(citas);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
