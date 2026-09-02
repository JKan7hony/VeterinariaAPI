using System;
using System.Collections.Generic;

namespace VeterinariaAPI.Models;

public partial class Factura
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public DateTime FechaEmision { get; set; }

    public decimal MontoImpuestos { get; set; }

    public decimal MontoTotal { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<DetallesFactura> DetallesFacturas { get; set; } = new List<DetallesFactura>();
}
