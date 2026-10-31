using RendicionesRegistroCivil.Data;
using RendicionesRegistroCivil.DTO.Rendicion;
using RendicionesRegistroCivil.Models;

namespace RendicionesRegistroCivil.Servicios
{
    public class RendicionService
    {
        private readonly AppDbContext _context;

        public RendicionService(AppDbContext context)
        {
            _context = context;
        }

        //Registrar una rendicion quincenal

        public void RegistrarRendicionQuincenal(RendicionDTO rendicionDTO)
        {
            Rendicione rendicion = new Rendicione
            {
                PeriodoInicio = rendicionDTO.PeriodoInicio,
                PeriodoFin = rendicionDTO.PeriodoFin,
            };

            _context.Rendiciones.Add(rendicion);
            _context.SaveChanges();
        }
    }
}
