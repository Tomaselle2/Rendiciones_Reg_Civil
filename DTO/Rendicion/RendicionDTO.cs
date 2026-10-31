using RendicionesRegistroCivil.Models;

namespace RendicionesRegistroCivil.DTO.Rendicion
{
    public class RendicionDTO
    {
        public int Id { get; set; }

        public DateOnly PeriodoInicio { get; set; }

        public DateOnly PeriodoFin { get; set; }

        public DateTime FechaCreacion { get; set; }

    }
}
