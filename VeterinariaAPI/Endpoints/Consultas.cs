using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class ConsultasApi
    {
        public static void MapConsultasApi(this WebApplication app)
        {
            var consultas = app.MapGroup("/api/consultas").WithTags("Consultas");

            //Api para listar consultas
            consultas.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaConsultas = await db.Consultas.ToListAsync();
                return Results.Ok(listaConsultas);
            });

            //API para crear una consulta
            consultas.MapPost("/", async (Consulta c, VeterinariodbContext db) =>
            {
                db.Consultas.Add(c);
                await db.SaveChangesAsync();
                return Results.Created($"/api/consultas/{c.Id}", c);
            });

            //API para editar consulta por ID
            consultas.MapPut("/{id:int}", async (int id, Consulta c, VeterinariodbContext db) =>
            {
                var consultas = await db.Consultas.FindAsync(id);
                if (consultas is null) return Results.NotFound();

                consultas.PacienteId = c.PacienteId;
                consultas.CitaId = c.CitaId;
                consultas.Motivo = c.Motivo;
                consultas.Diagnostico = c.Diagnostico;

                await db.SaveChangesAsync();
                return Results.Ok(consultas);
            });

            //API para eliminar consultas
            consultas.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var consultas = await db.Consultas.FindAsync(id);
                if (consultas is null) return Results.NotFound();

                db.Consultas.Remove(consultas);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
