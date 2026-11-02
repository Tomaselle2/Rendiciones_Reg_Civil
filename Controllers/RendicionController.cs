using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RendicionesRegistroCivil.DTO.Rendicion;
using RendicionesRegistroCivil.Interfaces;

namespace RendicionesRegistroCivil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RendicionController : ControllerBase
    {

        private readonly IRendicion _rendicionService;

        public RendicionController(IRendicion rendicionService)
        {
            _rendicionService = rendicionService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(21 * 1024 * 1024)]
        [ProducesResponseType(typeof(RendicionDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RendicionDTO>> Create([FromForm] CrearRendicionDTO dto)
        {
            var rendicion = await _rendicionService.Create(dto);

            return CreatedAtRoute("GetRendicionPorId", new { id = rendicion.Id }, rendicion);
        }

        [HttpGet("{id:int}", Name = "GetRendicionPorId")]
        [ProducesResponseType(typeof(RendicionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RendicionDTO>> GetById(int id)
        {
            var rendicion = await _rendicionService.GetbyId(id);

            if (rendicion is null)
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Rendición no encontrada",
                    Detail = $"No se encontró la rendición con ID {id}."
                });

            return Ok(rendicion);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RendicionDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RendicionDTO>>> GetAll(
             [FromQuery] int anio)
        {
            if (anio < 2000 || anio > DateTime.Now.Year)
                return BadRequest("El año ingresado no es válido.");

            var rendiciones = await _rendicionService.GetAll(anio);

            return Ok(rendiciones);
        }
    }
}
