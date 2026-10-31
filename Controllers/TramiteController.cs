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
    }
}
