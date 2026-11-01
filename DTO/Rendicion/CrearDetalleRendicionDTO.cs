using System.ComponentModel.DataAnnotations;

namespace RendicionesRegistroCivil.DTO.Rendicion
{
    public class CrearDetalleRendicionDTO
    {
        [Required]
        public int TramiteId { get; set; }

        [Range(0, int.MaxValue)]
        public int Cantidad { get; set; }
    }
}
