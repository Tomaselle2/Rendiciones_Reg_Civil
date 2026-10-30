using System;
using System.Collections.Generic;

namespace RendicionesRegistroCivil.Models;

public partial class DetalleRendicion
{
    public int Id { get; set; }

    public int TramiteId { get; set; }

    public int RendicionId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal? Subtotal { get; set; }

    public virtual Rendicione Rendicion { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;
}
