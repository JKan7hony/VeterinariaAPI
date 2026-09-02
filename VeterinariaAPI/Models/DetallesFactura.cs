using System;
using System.Collections.Generic;

namespace VeterinariaAPI.Models;

public partial class DetallesFactura
{
    public int Id { get; set; }

    public int FacturaId { get; set; }

    public int? ConsultaId { get; set; }

    public int? InsumoId { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Consulta? Consulta { get; set; }

    public virtual Factura Factura { get; set; } = null!;

    public virtual Insumo? Insumo { get; set; }
}
