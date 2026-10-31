using Microsoft.EntityFrameworkCore;
using RendicionesRegistroCivil.Data;
using RendicionesRegistroCivil.DTO.Tramite;
using RendicionesRegistroCivil.Interfaces;
using RendicionesRegistroCivil.Models;

namespace RendicionesRegistroCivil.Servicios
{
    public class TramiteService : ITramite
    {
        private readonly AppDbContext _context;

        public TramiteService(AppDbContext context)
        {
            _context = context;
        }

        //Registrar un tramite
        public void RegistrarTramite(TramiteDTO tramiteDTO)
        {
            Tramite tramite = new Tramite
            {
                Nombre = tramiteDTO.Nombre,
                Precio = tramiteDTO.Precio,
                TipoTramite = tramiteDTO.TipoTramite
            };

            _context.Tramites.Add(tramite);
            _context.SaveChanges();
        }


        //Consultar tramites
        public async Task<IEnumerable<TramiteDTO>> ConsultarTramites()
        {
            var tramites = await _context.Tramites.AsNoTracking().Select(t => new TramiteDTO
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Precio = t.Precio,
                TipoTramite = t.TipoTramite
            }).ToListAsync();

            return tramites;
        }
    }
}
