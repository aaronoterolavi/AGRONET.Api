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
        if (req is null)
            return BadRequest(new { message = "Body requerido." });

        var username = User.FindFirst("username")?.Value;

        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized(new { message = "Token inválido (username)." });

        if (string.IsNullOrWhiteSpace(req.Dni))
            return BadRequest(new { message = "El dni es requerido." });

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

        var cmd = new RegistrarMarcacionCommand
        {
            Dni = req.Dni,
            CodArea = req.CodArea,
            TipoAsistencia = req.TipoAsistencia,
            Nid = req.Nid,
            IdBarra = req.IdBarra,
            CodEmpresa = req.CodEmpresa,
            AudUsuarioLogin = username,
            AudIpMarca = ip
        };

        var result = await _service.RegistrarAsync(cmd, ct);

        if (result.Message.StartsWith("Error:", StringComparison.OrdinalIgnoreCase))
            return Conflict(new { message = result.Message });

        if (result.Message.Contains("Ud ya tiene registrado", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpGet("reporte")]
    public async Task<IActionResult> Reporte(string codArea, DateTime desde, DateTime hasta, CancellationToken ct)
        => Ok(await _service.ReporteAsync(codArea, desde, hasta, ct));

    [HttpPost("manual")]
    public async Task<IActionResult> Manual(RegistrarMarcacionManualRequest req, CancellationToken ct)
    {
        if (req is null)
            return BadRequest(new { message = "Body requerido." });

        var username = User.FindFirst("username")?.Value;

        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized(new { message = "Token inválido (username)." });

        var cmd = new RegistrarMarcacionManualCommand
        {
            dni = req.Dni,
            cod_area = req.CodArea,
            FechMarcaM = req.FechMarca,
            horaMarcaM = TimeSpan.Parse(req.HoraMarca),
            tipoAsistencia = req.TipoAsistencia,
            obsPapeleta = req.ObsPapeleta,
            codPapeleta = req.CodPapeleta,
            aud_UsuarioLogin = username,
            aud_ipMarca = HttpContext.Connection.RemoteIpAddress?.ToString() ?? ""
        };

        var result = await _service.RegistrarManualAsync(cmd, ct);

        if (!result.Success)
        {
            return result.Codigo switch
            {
                1001 => BadRequest(new { message = result.Message }), // Mes Cerrado
                1002 => Conflict(new { message = result.Message }),   // Duplicado
                1003 => Conflict(new { message = result.Message }),
                _ => BadRequest(new { message = result.Message })
            };
        }

        return Ok(new { message = result.Message });
    }



    [HttpGet("trabajadores")]
    public async Task<IActionResult> Trabajadores(string codArea,DateTime fecha, CancellationToken ct)
        => Ok(await _service.TrabajadoresAsync(codArea, fecha, ct));

    [HttpGet("listarAperturas")]
    public async Task<IActionResult> ListarAperturas([FromQuery] int? anio,CancellationToken ct)
    {
        var result = await _service.ListarAperturasAsync(anio, ct);
        return Ok(result);
    }

    [HttpPost("mantenerapertura")]
    public async Task<IActionResult> RegistrarAperturas([FromBody] RegistrarAperturaRequest request, CancellationToken ct)
    {
        if (request is null)
            return BadRequest(new { message = "Body requerido." });

        var username = User.FindFirst("username")?.Value;

        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized(new { message = "Token inválido (username)." });

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

        var cmd = new AperturaMarcacionDto
        {
            Anio = request.Anio,
            Mes = request.Mes,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Activo = request.Activo,
            Observacion = request.Observacion,
            Usuario = request.Usuario
        };

        var result = await _service.RegistrarAperturaAsync(cmd, ct);

        if (result.Message.StartsWith("Error:", StringComparison.OrdinalIgnoreCase))
            return Conflict(new { message = result.Message });

        //if (result.Message.Contains("Ud ya tiene registrado", StringComparison.OrdinalIgnoreCase))
        //    return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpGet("asistencia-mensual")]
    public async Task<IActionResult> AsistenciaMensual(
     [FromQuery] string mes,
     [FromQuery] string anio,
     CancellationToken ct)
    {
        var dni = User.FindFirst("dni")?.Value;

        if (string.IsNullOrWhiteSpace(dni))
            return Unauthorized(new { message = "Token inválido. No se encontró el DNI." });

        if (string.IsNullOrWhiteSpace(mes))
            return BadRequest(new { message = "El mes es requerido." });

        if (string.IsNullOrWhiteSpace(anio))
            return BadRequest(new { message = "El año es requerido." });

        mes = mes.Trim().PadLeft(2, '0');
        anio = anio.Trim();

        if (mes.Length != 2 || !int.TryParse(mes, out var mesNumero) || mesNumero < 1 || mesNumero > 12)
            return BadRequest(new { message = "El mes debe tener formato válido. Ejemplo: 07." });

        if (anio.Length != 4 || !int.TryParse(anio, out _))
            return BadRequest(new { message = "El año debe tener formato válido. Ejemplo: 2026." });

        var result = await _service.ListarAsistenciaMensualPorDniAsync(dni, mes, anio, ct);

        return Ok(result);
    }

}
