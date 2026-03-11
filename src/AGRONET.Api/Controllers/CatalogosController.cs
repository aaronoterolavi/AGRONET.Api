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
    }
}