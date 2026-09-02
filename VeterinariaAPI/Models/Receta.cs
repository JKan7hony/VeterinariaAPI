using System;
using System.Collections.Generic;

namespace VeterinariaAPI.Models;

public partial class Receta
{
    public int Id { get; set; }

    public int ConsultaId { get; set; }

    public DateOnly FechaEmision { get; set; }

    public DateOnly ValidaHasta { get; set; }

    public virtual Consulta Consulta { get; set; } = null!;

    public virtual ICollection<DetallesRecetum> DetallesReceta { get; set; } = new List<DetallesRecetum>();
}
