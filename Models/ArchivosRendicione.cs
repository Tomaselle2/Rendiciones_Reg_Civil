using System;
using System.Collections.Generic;

namespace RendicionesRegistroCivil.Models;

public partial class ArchivosRendicione
{
    public int Id { get; set; }

    public int RendicionId { get; set; }

    public string NombreOriginal { get; set; } = null!;

    public string NombreAlmacenado { get; set; } = null!;

    public string RutaRelativa { get; set; } = null!;

    public string TipoMime { get; set; } = null!;

    public long TamanoBytes { get; set; }

    public DateTime FechaCarga { get; set; }

    public virtual Rendicione Rendicion { get; set; } = null!;
}
