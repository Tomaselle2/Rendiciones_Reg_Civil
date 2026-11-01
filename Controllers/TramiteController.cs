using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RendicionesRegistroCivil.DTO.Tramite;
using RendicionesRegistroCivil.Interfaces;

namespace RendicionesRegistroCivil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TramiteController : ControllerBase
    {

        private readonly ITramite _tramiteService;

        public TramiteController(ITramite tramiteService)
        {
            _tramiteService = tramiteService;
        }

        [HttpGet(Name = "GetTramites")]
        [ProducesResponseType(typeof(IEnumerable<TramiteDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TramiteDTO>>> Get()
        {
            var tramites = await _tramiteService.ConsultarTramites();

            return Ok(tramites);
        }

        [HttpGet("{id:int}", Name = "GetTramitePorId")]
        [ProducesResponseType(typeof(TramiteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TramiteDTO>> GetById(int id)
        {
            var tramite = await _tramiteService.GetById(id);

            if (tramite is null)
                return NotFound();

            return Ok(tramite);
        }

        [HttpPost]
        public async Task<ActionResult<TramiteDTO>> Post(
        [FromBody] TramiteDTO tramiteDTO)
        {
            var registrado = await _tramiteService.RegistrarTramite(tramiteDTO);

            return CreatedAtAction(
                nameof(GetById),
                new { id = registrado.Id },
                registrado);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TramiteDTO>> Put(
            int id,
            [FromBody] TramiteDTO tramiteDTO)
        {
            if (id != tramiteDTO.Id)
                return BadRequest("El ID de la URL no coincide con el del trámite.");

            var editado = await _tramiteService.EditarTramite(tramiteDTO);

            return Ok(editado);
        }
    }
}
