using AGRONET.Menus.Application.Contracts;
using AGRONET.Menus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AGRONET.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/menus/admin")]
    public sealed class MenusAdminController : ControllerBase
    {
        private readonly IMenuAdminService _svc;

        public MenusAdminController(IMenuAdminService svc)
        {
            _svc = svc;
        }

        // GET /api/menus/admin?soloActivos=true
        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] bool soloActivos = true, CancellationToken ct = default)
        {
            var list = await _svc.ListarTodosAsync(soloActivos, ct);
            return Ok(list);
        }

        // GET /api/menus/admin/tree?soloActivos=true
        [HttpGet("tree")]
        public async Task<IActionResult> Tree([FromQuery] bool soloActivos = true, CancellationToken ct = default)
        {
            var tree = await _svc.ListarArbolAsync(soloActivos, ct);
            return Ok(tree);
        }

        // GET /api/menus/admin/{id}
        [HttpGet("{idMenu:int}")]
        public async Task<IActionResult> Obtener(int idMenu, CancellationToken ct)
        {
            var m = await _svc.ObtenerPorIdAsync(idMenu, ct);
            if (m is null) return NotFound(new { message = "Menú no existe." });
            return Ok(m);
        }

        // POST /api/menus/admin
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CreateMenuRequestDto req, CancellationToken ct)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.Nombre))
                return BadRequest(new { message = "Nombre es requerido." });

            var username = User.FindFirstValue("username") ?? "system";

            try
            {
                var idMenu = await _svc.CrearAsync(req, username, ct);
                return CreatedAtAction(nameof(Obtener), new { idMenu }, new { idMenu });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/menus/admin/{id}
        [HttpPut("{idMenu:int}")]
        public async Task<IActionResult> Actualizar(int idMenu, [FromBody] UpdateMenuRequestDto req, CancellationToken ct)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.Nombre))
                return BadRequest(new { message = "Nombre es requerido." });

            var username = User.FindFirstValue("username") ?? "system";

            try
            {
                await _svc.ActualizarAsync(idMenu, req, username, ct);
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/menus/admin/{id}/activo
        [HttpPut("{idMenu:int}/activo")]
        public async Task<IActionResult> CambiarActivo(int idMenu, [FromBody] ChangeActiveRequestDto req, CancellationToken ct)
        {
            var username = User.FindFirstValue("username") ?? "system";

            try
            {
                await _svc.CambiarActivoAsync(idMenu, req.Activo, username, ct);
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/menus/admin/{id}/orden
        [HttpPut("{idMenu:int}/orden")]
        public async Task<IActionResult> CambiarOrden(int idMenu, [FromBody] ChangeOrderRequestDto req, CancellationToken ct)
        {
            var username = User.FindFirstValue("username") ?? "system";

            try
            {
                await _svc.CambiarOrdenAsync(idMenu, req.Orden, username, ct);
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
