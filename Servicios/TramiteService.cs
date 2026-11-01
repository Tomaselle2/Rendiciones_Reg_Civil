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

        //Consultar todos los tramites
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

        //Editar un tramite
        public async Task<TramiteDTO> EditarTramite(TramiteDTO tramiteDTO)
        {
            var tramite = await _context.Tramites.FindAsync(tramiteDTO.Id);
            if (tramite == null)
            {
                throw new InvalidOperationException("Trámite no encontrado");
            }

            tramite.Nombre = tramiteDTO.Nombre;
            tramite.Precio = tramiteDTO.Precio;
            tramite.TipoTramite = tramiteDTO.TipoTramite;

            await _context.SaveChangesAsync();
            return tramiteDTO;
        }

        //consultar un tramite
        public async Task<TramiteDTO> GetById(int id)
        {
            var tramite = await _context.Tramites.AsNoTracking().Select(t => new TramiteDTO
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Precio = t.Precio,
                TipoTramite = t.TipoTramite
            }).FirstOrDefaultAsync(t => t.Id == id);

            if (tramite == null)
                throw new InvalidOperationException("Trámite no encontrado");

            return tramite;
        }

        //Registrar un tramite
        public async Task<TramiteDTO> RegistrarTramite(TramiteDTO tramiteDTO)
        {
            Tramite tramite = new Tramite
            {
                Nombre = tramiteDTO.Nombre,
                Precio = tramiteDTO.Precio,
                TipoTramite = tramiteDTO.TipoTramite
            };

            _context.Tramites.Add(tramite);
            await _context.SaveChangesAsync();

            tramiteDTO.Id = tramite.Id;

            return tramiteDTO;
        }
    }
}
