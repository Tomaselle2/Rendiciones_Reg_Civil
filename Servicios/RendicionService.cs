using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RendicionesRegistroCivil.Data;
using RendicionesRegistroCivil.DTO.Archivo;
using RendicionesRegistroCivil.DTO.Rendicion;
using RendicionesRegistroCivil.Interfaces;
using RendicionesRegistroCivil.Models;
using RendicionesRegistroCivil.Utils.Options;
using System.Security.Principal;

namespace RendicionesRegistroCivil.Servicios
{
    public class RendicionService : IRendicion
    {
        private readonly AppDbContext _context;
        private readonly string _storageRoot;


        public RendicionService(AppDbContext context, IOptions<StorageOptions> storage)
        {
            _context = context;
            _storageRoot = storage.Value.RootPath;
        }

        //metodo de negocio para crear una rendicion
        public async Task<RendicionDTO> Create(CrearRendicionDTO dto)
        {
            if (dto.Detalles is null || !dto.Detalles.Any())
                throw new ArgumentException(
                    "Debe ingresar al menos un detalle.");

            ValidarPeriodo(dto.PeriodoInicio, dto.PeriodoFin);
            ValidarArchivo(dto.Archivo);

            // Verificar período existente
            bool existe = await _context.Rendiciones
                .AnyAsync(r =>
                    r.PeriodoInicio == dto.PeriodoInicio &&
                    r.PeriodoFin == dto.PeriodoFin);

            if (existe)
                throw new InvalidOperationException(
                    "Ya existe una rendición para ese período.");

            // Verificar que no existan trámites repetidos
            if (dto.Detalles
                .GroupBy(d => d.TramiteId)
                .Any(g => g.Count() > 1))
            {
                throw new ArgumentException(
                    "No se puede repetir un trámite.");
            }

            // Obtener trámites del catálogo
            var tramites = await _context.Tramites
                .AsNoTracking()
                .ToListAsync();

            var cantidades = dto.Detalles.ToDictionary(
                d => d.TramiteId,
                d => d.Cantidad);

            // Verificar IDs inexistentes
            if (cantidades.Keys.Any(id =>
                tramites.All(t => t.Id != id)))
            {
                throw new ArgumentException(
                    "Se enviaron trámites inexistentes.");
            }

            // Crear rendición
            var rendicion = new Rendicione
            {
                Nombre = dto.Nombre,
                PeriodoInicio = dto.PeriodoInicio,
                PeriodoFin = dto.PeriodoFin,
                DetalleRendicions = new List<DetalleRendicion>()
            };

            // Generar detalles utilizando los precios del catálogo
            foreach (var tramite in tramites)
            {
                rendicion.DetalleRendicions.Add(
                    new DetalleRendicion
                    {
                        TramiteId = tramite.Id,

                        Cantidad = cantidades.GetValueOrDefault(
                            tramite.Id, 0),

                        PrecioUnitario = tramite.Precio
                    });
            }

            // --- Archivo ---
            var extension = Path.GetExtension(dto.Archivo.FileName).ToLowerInvariant();
            var nombreAlmacenado = $"{dto.Nombre.Trim()}_{Guid.NewGuid():N}{extension}";
            var rutaRelativa = Path.Combine("rendiciones",
                dto.PeriodoInicio.Year.ToString(), nombreAlmacenado);
            var rutaAbsoluta = Path.Combine(_storageRoot, rutaRelativa); // _storageRoot viene de config

            rendicion.ArchivosRendicione = new ArchivosRendicione
            {
                NombreOriginal = Path.GetFileName(dto.Archivo.FileName),
                NombreAlmacenado = nombreAlmacenado,
                RutaRelativa = rutaRelativa.Replace('\\', '/'),
                TipoMime = dto.Archivo.ContentType,
                TamanoBytes = dto.Archivo.Length
            };

            Directory.CreateDirectory(Path.GetDirectoryName(rutaAbsoluta)!);

            try
            {
                await using (var stream = new FileStream(rutaAbsoluta, FileMode.CreateNew))
                {
                    await dto.Archivo.CopyToAsync(stream);
                }

                // Persistir rendición y detalles
                _context.Rendiciones.Add(rendicion);

                await _context.SaveChangesAsync();   // rendición + detalles + archivo, atómico
            }
            catch
            {
                // Compensación: si falló la BD, no dejamos el archivo huérfano
                if (File.Exists(rutaAbsoluta))
                    File.Delete(rutaAbsoluta);
                throw;
            }

            // Devolver información registrada
            return await GetbyId(rendicion.Id);
        }

        public async Task<IEnumerable<RendicionDTO>> GetAll(int anio)
        {
            var rendiciones = await _context.Rendiciones
                .AsNoTracking()
                .Where(r => r.PeriodoInicio.Year == anio)
                .Include(r => r.ArchivosRendicione)
                .OrderBy(r => r.PeriodoInicio)
                .Select(r => new RendicionDTO
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    PeriodoInicio = r.PeriodoInicio,
                    PeriodoFin = r.PeriodoFin,
                    FechaCreacion = r.FechaCreacion,

                    Archivo = r.ArchivosRendicione != null ? new ArchivoRendicionDTO
                    {
                        Id = r.ArchivosRendicione.Id,
                        RendicionId = r.ArchivosRendicione.RendicionId,
                        NombreOriginal = r.ArchivosRendicione.NombreOriginal,
                        NombreAlmacenado = r.ArchivosRendicione.NombreAlmacenado,
                        RutaRelativa = r.ArchivosRendicione.RutaRelativa,
                        TipoMime = r.ArchivosRendicione.TipoMime,
                        TamanoBytes = r.ArchivosRendicione.TamanoBytes,
                        FechaCarga = r.ArchivosRendicione.FechaCarga
                    } : new ArchivoRendicionDTO()
                })
                .ToListAsync();

            return rendiciones;
        }

        public async Task<RendicionDTO> GetbyId(int id)
        {
            var rendicion = await _context.Rendiciones
                .Include(r => r.DetalleRendicions)
                .ThenInclude(d => d.Tramite)
                .Include(r => r.ArchivosRendicione)
                .AsNoTracking().Select(r => new RendicionDTO
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    PeriodoInicio = r.PeriodoInicio,
                    PeriodoFin = r.PeriodoFin,
                    DetalleRendicion = r.DetalleRendicions.Select(d => new DetalleRendicionDTO
                    {
                        Id = d.Id,
                        RendicionId = d.RendicionId,
                        TramiteId = d.TramiteId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList(),
                    Archivo = new ArchivoRendicionDTO
                    {
                        Id = r.ArchivosRendicione.Id,
                        RendicionId = r.ArchivosRendicione.RendicionId,
                        NombreOriginal = r.ArchivosRendicione.NombreOriginal,
                        NombreAlmacenado = r.ArchivosRendicione.NombreAlmacenado,
                        RutaRelativa = r.ArchivosRendicione.RutaRelativa,
                        TipoMime = r.ArchivosRendicione.TipoMime,
                        TamanoBytes = r.ArchivosRendicione.TamanoBytes,
                        FechaCarga = r.ArchivosRendicione.FechaCarga
                    }
                })
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rendicion is null)
            {
                return null;
            }
            else
            {
                return rendicion;
            }
        }

        public async Task<RendicionDTO?> Update(int id, RendicionDTO dto)
        {
            var rendicion = await _context.Rendiciones
                .Include(r => r.DetalleRendicions)
                .Include(r => r.ArchivosRendicione)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rendicion == null)
                return null;

            ValidarPeriodo(dto.PeriodoInicio, dto.PeriodoFin);

            // Actualizar datos principales
            rendicion.Nombre = dto.Nombre;
            rendicion.PeriodoInicio = dto.PeriodoInicio;
            rendicion.PeriodoFin = dto.PeriodoFin;

            // Materializar los detalles recibidos
            var detallesDTO = dto.DetalleRendicion.ToList();

            // Validar que no se repitan trámites
            if (detallesDTO.Select(d => d.TramiteId).Distinct().Count()
                != detallesDTO.Count)
            {
                throw new ArgumentException(
                    "No se puede repetir un trámite en una rendición.");
            }

            var detallesActuales = rendicion.DetalleRendicions.ToList();

            // Actualizar o agregar detalles
            foreach (var detalleDTO in detallesDTO)
            {
                if (detalleDTO.Id == 0)
                {
                    rendicion.DetalleRendicions.Add(new DetalleRendicion
                    {
                        TramiteId = detalleDTO.TramiteId,
                        Cantidad = detalleDTO.Cantidad,
                        PrecioUnitario = detalleDTO.PrecioUnitario
                    });

                    continue;
                }

                var detalleExistente = detallesActuales
                    .FirstOrDefault(d => d.Id == detalleDTO.Id);

                if (detalleExistente == null)
                {
                    throw new ArgumentException(
                        $"El detalle {detalleDTO.Id} no pertenece a esta rendición.");
                }

                detalleExistente.TramiteId = detalleDTO.TramiteId;
                detalleExistente.Cantidad = detalleDTO.Cantidad;
                detalleExistente.PrecioUnitario = detalleDTO.PrecioUnitario;
            }

            // Eliminar detalles que ya no vienen en el DTO
            var idsRecibidos = detallesDTO
                .Where(d => d.Id != 0)
                .Select(d => d.Id)
                .ToHashSet();

            var detallesEliminar = detallesActuales
                .Where(d => !idsRecibidos.Contains(d.Id))
                .ToList();

            _context.DetalleRendicions.RemoveRange(detallesEliminar);

            await _context.SaveChangesAsync();

            // Retornar la rendición actualizada
            return await GetbyId(id);
        }

        private static void ValidarPeriodo(
            DateOnly inicio,
            DateOnly fin)
        {
            // La fecha de inicio no puede ser posterior a la fecha de fin.
            if (inicio > fin)
            {
                throw new ArgumentException(
                    "La fecha de inicio no puede ser posterior a la fecha de fin.");
            }

            // Ambas fechas deben pertenecer al mismo mes y año.
            if (inicio.Month != fin.Month || inicio.Year != fin.Year)
            {
                throw new ArgumentException(
                    "El período debe pertenecer al mismo mes.");
            }

            // Primera quincena: días 1 al 15.
            bool primeraQuincena =
                inicio.Day <= 15 &&
                fin.Day <= 15;

            // Segunda quincena: días 16 al último día del mes.
            bool segundaQuincena =
                inicio.Day >= 16 &&
                fin.Day >= 16;

            if (!primeraQuincena && !segundaQuincena)
            {
                throw new ArgumentException(
                    "El período debe corresponder a una única quincena.");
            }
        }

        private const long MaxArchivoBytes = 20 * 1024 * 1024;
        private static readonly string[] ExtensionesPermitidas = { ".xlsx", ".xls" };

        private static void ValidarArchivo(IFormFile? archivo)
        {
            if (archivo is null || archivo.Length == 0)
                throw new ArgumentException("Debe adjuntar un archivo Excel.");

            if (archivo.Length > MaxArchivoBytes)
                throw new ArgumentException("El archivo no puede superar los 20 MB.");

            var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!ExtensionesPermitidas.Contains(ext))
                throw new ArgumentException("Solo se permiten archivos Excel (.xlsx, .xls).");
        }
    }
}
