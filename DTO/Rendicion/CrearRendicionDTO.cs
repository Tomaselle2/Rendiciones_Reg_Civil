using System.ComponentModel.DataAnnotations;

namespace RendicionesRegistroCivil.DTO.Rendicion
{
    public class CrearRendicionDTO
    {
        [Required(ErrorMessage = "La fecha de inicio es requerida.")]
        public DateOnly PeriodoInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida.")]
        public DateOnly PeriodoFin { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe ingresar al menos un trámite.")]
        public List<CrearDetalleRendicionDTO> Detalles { get; set; } = new();

        [Required(ErrorMessage = "Debe adjuntar el archivo Excel.")]
        public IFormFile Archivo { get; set; } = null!;
    }
}
