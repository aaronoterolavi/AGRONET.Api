using AGRONET.Users.Application.Contracts;
using AGRONET.Users.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGRONET.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/users/admin")]
    public sealed class UsersAdminController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersAdminController(IUserService service)
        {
            _service = service;
        }

        // GET /api/users/admin?soloActivos=true
        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] bool soloActivos = true, CancellationToken ct = default)
        {
            var list = await _service.ListarAsync(soloActivos, ct);
            return Ok(list);
        }

        // GET /api/users/admin/{idUsuario}
        [HttpGet("{idUsuario:int}")]
        public async Task<IActionResult> Obtener(int idUsuario, CancellationToken ct)
        {
            var user = await _service.ObtenerPorIdAsync(idUsuario, ct);

            if (user is null)
                return NotFound(new { message = "Usuario no existe." });

            return Ok(user);
        }

        // POST /api/users/admin
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CreateUserRequestDto request, CancellationToken ct)
        {
            try
            {
                var idUsuario = await _service.CrearAsync(request, ct);
                return CreatedAtAction(nameof(Obtener), new { idUsuario }, new { idUsuario });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/users/admin/{idUsuario}
        [HttpPut("{idUsuario:int}")]
        public async Task<IActionResult> Actualizar(int idUsuario, [FromBody] UpdateUserRequestDto request, CancellationToken ct)
        {
            try
            {
                await _service.ActualizarAsync(idUsuario, request, ct);
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/users/admin/{idUsuario}/activo
        [HttpPut("{idUsuario:int}/activo")]
        public async Task<IActionResult> CambiarActivo(int idUsuario, [FromBody] ChangeUserActiveRequestDto request, CancellationToken ct)
        {
            try
            {
                await _service.CambiarActivoAsync(idUsuario, request.Activo, ct);
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}