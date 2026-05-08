using AGRONET.Catalogos.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGRONET.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/catalogos")]
    public sealed class CatalogosController : ControllerBase
    {
        private readonly ICatalogosService _service;

        public CatalogosController(ICatalogosService service)
        {
            _service = service;
        }

        // GET /api/catalogos/areas
        [HttpGet("areas")]
        public async Task<IActionResult> ListarAreas(CancellationToken ct)
        {
            var list = await _service.ListarAreasAsync(ct);
            return Ok(list);
        }
        // GET /api/catalogos/areas/padre
        [HttpGet("areas/padre")]
        public async Task<IActionResult> ListarAreasPadre(CancellationToken ct)
        {
            var list = await _service.ListarAreasPadreAsync(ct);
            return Ok(list);
        }
        // GET /api/catalogos/areas/hijas?codPadre={codPadre}
        [HttpGet("areas/hijas")]
        public async Task<IActionResult> ListarAreasHijas([FromQuery] string codPadre, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(codPadre))
            {
                return BadRequest(new { mensaje = "El código padre es requerido" });
            }

            var list = await _service.ListarAreasHijasAsync(codPadre, ct);
            return Ok(list);
        }

    }
}