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
        public async Task<ActionResult<RendicionDTO>> Create([FromForm] CrearRendicionDTO dto)
        {
            // Aquí iría la lógica para crear la rendición utilizando el servicio correspondiente
            // Por ejemplo:
            // var rendicionCreada = await _rendicionService.Create(dto);
            // return CreatedAtAction(nameof(GetById), new { id = rendicionCreada.Id }, rendicionCreada);
            return Ok(); // Placeholder response
        }

        [HttpGet("{id:int}", Name = "GetRendicionPorId")]
        [ProducesResponseType(typeof(RendicionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RendicionDTO>> GetById(int id)
        {
            var rendicion = await _rendicionService.GetbyId(id);

            if(rendicion is null)
                return NotFound();

            return Ok(rendicion);
        }
    }
}
