using VeterinariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Endpoints
{
    public static class PacientesApi
    {
        public static void MapPacientesApi(this WebApplication app)
        {
            var pacientes = app.MapGroup("/api/pacientes").WithTags("pacientes");

            //Api para listar pacientes
            pacientes.MapGet("/", async (VeterinariodbContext db) =>
            {
                var listaPacientes = await db.Pacientes.ToListAsync();
                return Results.Ok(listaPacientes);
            });

            //API para crear un paciente
            pacientes.MapPost("/", async (Paciente p, VeterinariodbContext db) =>
            {
                db.Pacientes.Add(p);
                await db.SaveChangesAsync();
                return Results.Created($"/api/pacientes/{p.Id}", p);
            });

            //API para editar paciente por ID
            pacientes.MapPut("/{id:int}", async (int id, Paciente p, VeterinariodbContext db) =>
            {
                var pacientes = await db.Pacientes.FindAsync(id);
                if (pacientes is null) return Results.NotFound();

                pacientes.ClienteId = p.ClienteId;
                pacientes.Nombre = p.Nombre;
                pacientes.Especie = p.Especie;
                pacientes.Raza = p.Raza;
                pacientes.Peso = p.Peso;
                pacientes.Alergias = p.Alergias;

                await db.SaveChangesAsync();
                return Results.Ok(pacientes);
            });

            //API para eliminar pacientes
            pacientes.MapDelete("/{id:int}", async (int id, VeterinariodbContext db) =>
            {
                var pacientes = await db.Pacientes.FindAsync(id);
                if (pacientes is null) return Results.NotFound();

                db.Pacientes.Remove(pacientes);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
