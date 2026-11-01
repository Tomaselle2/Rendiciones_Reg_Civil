using RendicionesRegistroCivil.Utils.Enum;
using System.ComponentModel.DataAnnotations;

namespace RendicionesRegistroCivil.DTO.Tramite
{
    public class TramiteDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del trámite es obligatorio.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = null!;


        [PrecioValido]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El tipo de trámite es obligatorio.")]
        [EnumString<TipoTramite>(
        ErrorMessage = "El tipo debe ser DNI o PASAPORTE.")]
        public string TipoTramite { get; set; } = string.Empty;
    }

    public class EnumStringAttribute<TEnum> : ValidationAttribute
    where TEnum : struct, Enum
    {
        public override bool IsValid(object? value)
        {
            if (value is not string texto)
                return false;

            return Enum.TryParse<TEnum>(texto, ignoreCase: false, out var resultado)
                && Enum.IsDefined(resultado)
                && resultado.ToString() == texto;
        }
    }

    public class PrecioValidoAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not decimal precio)
                return false;

            return precio >= 0m && precio <= 9999999999.99m;
        }
    }
}
