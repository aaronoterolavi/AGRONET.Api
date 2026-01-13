using AGRONET.Auth.Application.Contracts.Auth;
using AGRONET.Auth.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AGRONET.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        // POST: /api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto req, CancellationToken ct)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new LoginResponseDto { Status = "ERROR", Message = "Username y Password son requeridos." });

            var (ip, ua) = GetClientInfo();

            var result = await _auth.LoginAsync(req.Username, req.Password, ip, ua, ct);

            // Armamos respuesta tipo DTO (reutilizamos tu LoginResponseDto)
            var res = new LoginResponseDto
            {
                Status = result.Status,
                Message = result.Message,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                RegistrationToken = result.RegistrationToken,
                User = result.User
            };

            // Si AD inválido o error -> 401/400 según prefieras. Yo uso 401 para credenciales.
            if (result.Status == "ERROR" && (result.Message?.Contains("incorrectos", StringComparison.OrdinalIgnoreCase) ?? false))
                return Unauthorized(res);

            return Ok(res);
        }

        // POST: /api/auth/complete-registration
        [AllowAnonymous]
        [HttpPost("complete-registration")]
        public async Task<ActionResult<LoginResponseDto>> CompleteRegistration([FromBody] CompleteRegistrationRequestDto req, CancellationToken ct)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.RegistrationToken) || string.IsNullOrWhiteSpace(req.Dni))
                return BadRequest(new LoginResponseDto { Status = "ERROR", Message = "RegistrationToken y DNI son requeridos." });

            var (ip, ua) = GetClientInfo();

            var result = await _auth.CompleteRegistrationAsync(req.RegistrationToken, req.Dni, ip, ua, ct);

            var res = new LoginResponseDto
            {
                Status = result.Status,
                Message = result.Message,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                RegistrationToken = result.RegistrationToken,
                User = result.User
            };

            // Si DNI no existe, mejor devolver 404 (pero tu result llega como ERROR). Lo detectamos por mensaje.
            if (result.Status == "ERROR" && (result.Message?.Contains("No se encontró el DNI", StringComparison.OrdinalIgnoreCase) ?? false))
                return NotFound(res);

            // Token de registro inválido -> 401
            if (result.Status == "ERROR" && (result.Message?.Contains("Token de registro inválido", StringComparison.OrdinalIgnoreCase) ?? false))
                return Unauthorized(res);

            return Ok(res);
        }

        // POST: /api/auth/refresh
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponseDto>> Refresh([FromBody] RefreshRequestDto req, CancellationToken ct)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.RefreshToken))
                return BadRequest(new LoginResponseDto { Status = "ERROR", Message = "RefreshToken es requerido." });

            var (ip, ua) = GetClientInfo();

            var result = await _auth.RefreshAsync(req.RefreshToken, ip, ua, ct);

            var res = new LoginResponseDto
            {
                Status = result.Status,
                Message = result.Message,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                RegistrationToken = result.RegistrationToken,
                User = result.User
            };

            if (result.Status == "ERROR")
                return Unauthorized(res);

            return Ok(res);
        }

        // POST: /api/auth/logout

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto req, CancellationToken ct)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.RefreshToken))
                return BadRequest(new { message = "RefreshToken es requerido." });

            // ✅ 1) Intenta NameIdentifier (lo más común)
            var idStr =
                 User.FindFirstValue(ClaimTypes.NameIdentifier) // ✅ preferido
                 ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                 ?? User.FindFirstValue("sub")
                 ?? User.FindFirstValue("nameid");

            if (!int.TryParse(idStr, out var idUsuario))
                return Unauthorized(new { message = "Token inválido (id)." });

            await _auth.LogoutAsync(idUsuario, req.RefreshToken, ct);
            return Ok(new { message = "OK" });
        }

        private (string? ip, string? userAgent) GetClientInfo()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua = Request.Headers.UserAgent.ToString();
            if (string.IsNullOrWhiteSpace(ua)) ua = null;
            return (ip, ua);
        }
    }
}
