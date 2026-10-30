using System;
using System.Collections.Generic;

namespace RendicionesRegistroCivil.Models;

public partial class Tramite
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public string TipoTramite { get; set; } = null!;

    public virtual ICollection<DetalleRendicion> DetalleRendicions { get; set; } = new List<DetalleRendicion>();
}
