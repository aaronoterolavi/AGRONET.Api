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
    [Route("api/roles/{idRol:int}/menus")]
    public sealed class RoleMenusController : ControllerBase
    {
        private readonly IMenuService _menus;

        public RoleMenusController(IMenuService menus)
        {
            _menus = menus;
        }

        // GET /api/roles/{idRol}/menus  => árbol del rol (admin)
        [HttpGet]
        public async Task<IActionResult> Get(int idRol, CancellationToken ct)
        {
            var tree = await _menus.ObtenerArbolPorRolAsync(idRol, ct);
            return Ok(tree);
        }

        // PUT /api/roles/{idRol}/menus  => reemplazar asignaciones
        [HttpPut]
        public async Task<IActionResult> Replace(int idRol, [FromBody] ReplaceRoleMenusRequestDto req, CancellationToken ct)
        {
            if (req is null) return BadRequest(new { message = "Body requerido." });

            var username = User.FindFirstValue("username") ?? "system";

            await _menus.ReemplazarMenusDeRolAsync(idRol, req.MenuIds, username, ct);
            return Ok(new { message = "OK" });
        }
    }
}
