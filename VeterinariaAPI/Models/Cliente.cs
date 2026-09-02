using System;
using System.Collections.Generic;

namespace VeterinariaAPI.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string DocumentoIdentidad { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();
}
