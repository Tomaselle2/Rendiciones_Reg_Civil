namespace RendicionesRegistroCivil.DTO.Tramite
{
    public class TramiteDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public decimal Precio { get; set; }

        public string TipoTramite { get; set; } = null!;
    }
}
