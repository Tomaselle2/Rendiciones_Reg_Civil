using System;
using System.Collections.Generic;

namespace RendicionesRegistroCivil.Models;

public partial class Rendicione
{
    public int Id { get; set; }

    public DateOnly PeriodoInicio { get; set; }

    public DateOnly PeriodoFin { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ArchivosRendicione? ArchivosRendicione { get; set; }

    public virtual ICollection<DetalleRendicion> DetalleRendicions { get; set; } = new List<DetalleRendicion>();
}
