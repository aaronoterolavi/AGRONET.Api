using AGRONET.Menus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AGRONET.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public sealed class MenusController : ControllerBase
    {
        private readonly IMenuService _menus;

        public MenusController(IMenuService menus)
        {
            _menus = menus;
        }

        // GET: /api/menus
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            // ✅ Lee el roleId que ya agregaste al token
            var roleIdStr = User.FindFirstValue("roleId");
            if (!int.TryParse(roleIdStr, out var idRol))
                return Unauthorized(new { message = "Token inválido (roleId)." });

            var tree = await _menus.ObtenerArbolPorRolAsync(idRol, ct);
            return Ok(tree);
        }
    }
}
