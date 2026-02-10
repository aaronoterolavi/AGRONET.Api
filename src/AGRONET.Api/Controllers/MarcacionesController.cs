using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AGRONET.Marcacion.Application.Contracts;
using AGRONET.Marcacion.Application.Services;

namespace AGRONET.Api.Controllers;

[ApiController]
[Route("api/marcaciones")]
[Authorize]
public sealed class MarcacionesController : ControllerBase
{
    private readonly MarcacionService _service;

    public MarcacionesController(MarcacionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarMarcacionRequest req, CancellationToken ct)
    {
        if (req is null) return BadRequest(new { message = "Body requerido." });

        // Claims emitidos por TU TokenService
        var username = User.FindFirst("username")?.Value;
        var dni = User.FindFirst("dni")?.Value;

        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized(new { message = "Token inválido (username)." });

        if (string.IsNullOrWhiteSpace(dni))
            return Unauthorized(new { message = "Token inválido (dni)." });

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

        var cmd = new RegistrarMarcacionCommand
        {
            Dni = dni,
            CodArea = req.CodArea,
            TipoAsistencia = req.TipoAsistencia,
            Nid = req.Nid,
            IdBarra = req.IdBarra,
            CodEmpresa = req.CodEmpresa,
            AudUsuarioLogin = username,
            AudIpMarca = ip
        };

        var result = await _service.RegistrarAsync(cmd, ct);

        // Mapeo simple de respuestas (ajustable)
        if (result.Message.StartsWith("Error:", StringComparison.OrdinalIgnoreCase))
            return Conflict(new { message = result.Message });

        if (result.Message.Contains("Ud ya tiene registrado", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }
}
