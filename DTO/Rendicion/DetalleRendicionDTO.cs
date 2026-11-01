namespace RendicionesRegistroCivil.DTO.Rendicion
{
    public class DetalleRendicionDTO
    {
        public int Id { get; set; }

        public int TramiteId { get; set; }

        public int RendicionId { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal? Subtotal { get; set; }
    }
}
