using AGRONET.Roles.Application.Contracts;
using AGRONET.Roles.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGRONET.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    public sealed class RolesController : ControllerBase
    {
        private readonly IRoleService _roles;

        public RolesController(IRoleService roles)
        {
            _roles = roles;
        }

        // GET /api/roles
        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> Listar(CancellationToken ct)
        {
            var list = await _roles.ListarAsync(ct);
            return Ok(list);
        }

        // GET /api/roles/{id}
        [HttpGet("{idRol:int}")]
        public async Task<ActionResult<RoleDto>> Obtener(int idRol, CancellationToken ct)
        {
            var rol = await _roles.ObtenerPorIdAsync(idRol, ct);
            if (rol is null) return NotFound(new { message = "Rol no existe." });
            return Ok(rol);
        }

        // POST /api/roles
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CreateRoleRequestDto req, CancellationToken ct)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.Codigo) || string.IsNullOrWhiteSpace(req.Nombre))
                return BadRequest(new { message = "Codigo y Nombre son requeridos." });

            var idRol = await _roles.CrearAsync(req, ct);
            return CreatedAtAction(nameof(Obtener), new { idRol }, new { idRol });
        }

        // PUT /api/roles/{id}
        [HttpPut("{idRol:int}")]
        public async Task<IActionResult> Actualizar(int idRol, [FromBody] UpdateRoleRequestDto req, CancellationToken ct)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.Nombre))
                return BadRequest(new { message = "Nombre es requerido." });

            try
            {
                await _roles.ActualizarAsync(idRol, req, ct);
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                // si quieres, aquí mapeas mensajes del SP a 400/409
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/roles/{id}/activo
        [HttpPut("{idRol:int}/activo")]
        public async Task<IActionResult> CambiarActivo(int idRol, [FromBody] ChangeRoleActiveRequestDto req, CancellationToken ct)
        {
            try
            {
                await _roles.CambiarActivoAsync(idRol, req.Activo, ct);
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
