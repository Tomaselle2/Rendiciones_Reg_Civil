using RendicionesRegistroCivil.DTO.Archivo;
using RendicionesRegistroCivil.Models;
using System.ComponentModel.DataAnnotations;

namespace RendicionesRegistroCivil.DTO.Rendicion
{
    public class RendicionDTO
    {
        public int Id { get; set; }

        public DateOnly PeriodoInicio { get; set; }

        public DateOnly PeriodoFin { get; set; }

        public DateTime FechaCreacion { get; set; }

        public IEnumerable<DetalleRendicionDTO> DetalleRendicion { get; set; } = new List<DetalleRendicionDTO>();

        public ArchivoRendicionDTO Archivo { get; set; } = new ArchivoRendicionDTO();

    }
}
