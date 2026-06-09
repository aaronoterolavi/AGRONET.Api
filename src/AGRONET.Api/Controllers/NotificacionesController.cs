using AGRONET.Notificaciones.Application.DTOs;
using AGRONET.Notificaciones.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using System.Net;

namespace AGRONET.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionesService _service;
    private readonly IConfiguration _configuration;

    public NotificacionesController(INotificacionesService service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    private string? ObtenerDniDesdeToken() => User.FindFirst("dni")?.Value ?? User.FindFirstValue("dni");

    [HttpGet("catalogos")]
    public async Task<IActionResult> Catalogos(CancellationToken ct) => Ok(await _service.ListarCatalogosAsync(ct));

    [HttpGet("personal")]
    public async Task<IActionResult> Personal([FromQuery] string? texto, CancellationToken ct) => Ok(await _service.BuscarPersonalAsync(texto, ct));

    [HttpGet("registro")]
    public async Task<IActionResult> Registro([FromQuery] string? estado, [FromQuery] string? tipoDocumento, [FromQuery] string? buscar, CancellationToken ct)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni)) return Unauthorized(new { message = "Token sin claim 'dni'." });
        return Ok(await _service.ListarRegistroAsync(dni, estado, tipoDocumento, buscar, ct));
    }

    [HttpGet("casilla")]
    public async Task<IActionResult> Casilla([FromQuery] string? estado, [FromQuery] string? buscar, CancellationToken ct)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni)) return Unauthorized(new { message = "Token sin claim 'dni'." });
        return Ok(await _service.ListarCasillaAsync(dni, estado, buscar, ct));
    }

    [HttpGet("reporte")]
    public async Task<IActionResult> Reporte([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin, [FromQuery] string? estado, [FromQuery] string? buscar, CancellationToken ct)
        => Ok(await _service.ReporteAsync(fechaInicio, fechaFin, estado, buscar, ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken ct)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni)) return Unauthorized(new { message = "Token sin claim 'dni'." });
        var data = await _service.ObtenerDetalleAsync(id, dni, ct);
        return data is null ? NotFound(new { message = "No se encontró la notificación." }) : Ok(data);
    }

    [HttpPost("borrador")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> GuardarBorrador([FromForm] NotificacionGuardarForm form, CancellationToken ct)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni)) return Unauthorized(new { message = "Token sin claim 'dni'." });
        var res = await _service.GuardarBorradorAsync(dni, ToDto(form), ct);
        if (res.Codigo != 1 || res.Id is null) return UnprocessableEntity(res);
        await GuardarDocumentosAsync(res.Id.Value, form.DocumentoPrincipal, form.Anexos, ct);
        return Ok(res);
    }

    [HttpPost("publicar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Publicar([FromForm] NotificacionGuardarForm form, CancellationToken ct)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni)) return Unauthorized(new { message = "Token sin claim 'dni'." });
        var res = await _service.PublicarAsync(dni, ToDto(form), ct);
        if (res.Codigo != 1 || res.Id is null) return UnprocessableEntity(res);
        await GuardarDocumentosAsync(res.Id.Value, form.DocumentoPrincipal, form.Anexos, ct);
        return Ok(res);
    }

    [HttpPut("{id:int}/anular")]
    public async Task<IActionResult> Anular(int id, [FromBody] NotificacionAnularRequest req, CancellationToken ct)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni)) return Unauthorized(new { message = "Token sin claim 'dni'." });
        var res = await _service.AnularAsync(id, dni, req.Motivo ?? "", ct);
        return res.Codigo == 1 ? Ok(res) : UnprocessableEntity(res);
    }

    [HttpPut("{id:int}/acuse")]
    public async Task<IActionResult> Acuse(int id, CancellationToken ct)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni)) return Unauthorized(new { message = "Token sin claim 'dni'." });
        var res = await _service.AceptarAcuseAsync(id, dni, ct);
        return res.Codigo == 1 ? Ok(res) : UnprocessableEntity(res);
    }

    [HttpPost("{id:int}/respuesta")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Responder(int id, [FromForm] NotificacionRespuestaForm form, CancellationToken ct)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni)) return Unauthorized(new { message = "Token sin claim 'dni'." });
        var res = await _service.RegistrarRespuestaAsync(id, dni, new NotificacionRespuestaGuardarDto { Comentario = form.Comentario }, ct);
        if (res.Codigo != 1) return UnprocessableEntity(res);
        var idRespuesta = await _service.ObtenerIdRespuestaAsync(id, dni, ct);
        if (idRespuesta is not null)
        {
            await GuardarDocumentosRespuestaAsync(idRespuesta.Value, form.DocumentoPrincipal, form.Anexos, ct);
        }
        return Ok(res);
    }


    [HttpGet("{id:int}/constancia")]
    public async Task<IActionResult> Constancia(int id, CancellationToken ct)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni)) return Unauthorized(new { message = "Token sin claim 'dni'." });

        var n = await _service.ObtenerDetalleAsync(id, dni, ct);
        if (n is null) return NotFound(new { message = "No se encontró la notificación." });
        if (!string.Equals(n.Destinatario?.Dni, dni, StringComparison.OrdinalIgnoreCase))
            return Forbid();
        if (n.Respuesta is null)
            return UnprocessableEntity(new { message = "La constancia solo puede generarse cuando el destinatario ya registró una respuesta." });

        static string H(string? value) => WebUtility.HtmlEncode(value ?? "-");
        var fechaGeneracion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        var fechaPublicacion = n.FechaPublicacion?.ToString("dd/MM/yyyy HH:mm:ss") ?? "-";
        var fechaEmision = n.FechaEmision?.ToString("dd/MM/yyyy") ?? "-";
        var fechaAcuse = n.FechaAcuse?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Pendiente";
        var estadoAcuse = n.AcuseAceptado ? "ACUSE ACEPTADO" : "PENDIENTE DE ACUSE";

        var html = $@"<!doctype html>
<html lang='es'>
<head>
<meta charset='utf-8'>
<title>Constancia de Notificación {H(n.Codigo)}</title>
<style>
body {{ font-family: Arial, sans-serif; color:#222; margin:26px 40px 90px 40px; font-size:13px; }}
.header {{ text-align:center; border-bottom:2px solid #1b5e20; padding-bottom:10px; margin-bottom:14px; }}
.header h1 {{ color:#1b5e20; font-size:22px; margin:0; }}
.sub {{ font-size:12px; color:#666; }}
.section {{ margin-top:10px; page-break-inside:avoid; }}
.label {{ font-weight:bold; color:#1b5e20; }}
table {{ width:100%; border-collapse:collapse; margin-top:7px; page-break-inside:avoid; }}
td {{ border:1px solid #ddd; padding:6px; vertical-align:top; }}
.msg {{ border:1px solid #ddd; background:#f8f9fa; padding:8px; white-space:pre-wrap; }}
.footer {{ font-size:10px; color:#555; margin-top:18px; }}
.sign {{ text-align:center; font-size:11px; color:#333; margin-top:28px; page-break-inside:avoid; }}
.btn-print {{ position:fixed; top:15px; right:15px; padding:9px 14px; background:#1b5e20; color:white; border:0; border-radius:4px; }}
@page {{ size: A4; margin: 15mm 18mm 22mm 18mm; }}
@media print {{
  .btn-print {{ display:none; }}
  body {{ margin:0 0 28mm 0; font-size:11px; }}
  .header {{ padding-bottom:8px; margin-bottom:10px; }}
  .section {{ margin-top:8px; }}
  td {{ padding:5px; }}
  .msg {{ padding:6px; }}
  .footer {{ position:fixed; bottom:12mm; left:0; right:0; font-size:9px; }}
  .sign {{ position:fixed; bottom:4mm; left:0; right:0; margin-top:0; font-size:10px; }}
}}
</style>
</head>
<body>
<button class='btn-print' onclick='window.print()'>Imprimir / Guardar PDF</button>
<div class='header'>
  <h1>CONSTANCIA DE NOTIFICACIÓN</h1>
  <div class='sub'>Módulo de Notificaciones - Agro Rural</div>
</div>
<table>
<tr><td><span class='label'>Código:</span><br>{H(n.Codigo)}</td><td><span class='label'>Estado:</span><br>{H(n.Estado)}</td></tr>
<tr><td><span class='label'>Tipo de documento:</span><br>{H(n.TipoDocumento)}</td><td><span class='label'>Fecha de emisión:</span><br>{H(fechaEmision)}</td></tr>
<tr><td><span class='label'>Fecha de publicación/envío:</span><br>{H(fechaPublicacion)}</td><td><span class='label'>Fecha de generación:</span><br>{H(fechaGeneracion)}</td></tr>
<tr><td><span class='label'>Remitente:</span><br>{H(n.Remitente.NombreCompleto)}<br><small>{H(n.Remitente.Cargo)}</small></td><td><span class='label'>Destinatario:</span><br>{H(n.Destinatario.NombreCompleto)}<br><small>DNI: {H(n.Destinatario.Dni)}<br>{H(n.Destinatario.Cargo)}</small></td></tr>
<tr><td><span class='label'>Acuse:</span><br>{H(estadoAcuse)}</td><td><span class='label'>Fecha de acuse:</span><br>{H(fechaAcuse)}</td></tr>
</table>
<div class='section'>
  <div class='label'>Asunto:</div>
  <div>{H(n.Asunto)}</div>
</div>
<div class='section'>
  <div class='label'>Mensaje:</div>
  <div class='msg'>{H(n.Mensaje)}</div>
</div>
<div class='section'>
  <div class='label'>Documento principal:</div>
  <div>{H(n.DocumentoPrincipal?.Nombre)}</div>
</div>
<div class='section'>
  <div class='label'>Anexos:</div>
  <div>{H(string.Join(", ", n.Anexos.Select(a => a.Nombre)))}</div>
</div>
<div class='section'>
  <div class='label'>Respuesta del destinatario:</div>
  <table>
    <tr><td><span class='label'>Fecha de respuesta:</span><br>{H(n.Respuesta?.Fecha?.ToString("dd/MM/yyyy HH:mm:ss"))}</td><td><span class='label'>Documento de respuesta:</span><br>{H(n.Respuesta?.DocumentoPrincipal?.Nombre)}</td></tr>
    <tr><td colspan='2'><span class='label'>Comentario:</span><br>{H(n.Respuesta?.Comentario)}</td></tr>
  </table>
</div>
<div class='footer'>
La presente constancia se genera automáticamente con la información registrada en el Sistema de Notificaciones.
</div>
<div class='sign'>____________________________________<br>Constancia generada por el sistema</div>
<script>window.onload = function(){{ setTimeout(function(){{ window.print(); }}, 500); }};</script>
</body>
</html>";
        return Content(html, "text/html", Encoding.UTF8);
    }

    [HttpGet("documentos/{idDocumento:int}/archivo")]
    public async Task<IActionResult> DescargarDocumento(int idDocumento, CancellationToken ct)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni)) return Unauthorized(new { message = "Token sin claim 'dni'." });
        var doc = await _service.ObtenerDocumentoAsync(idDocumento, dni, ct);
        if (doc is null) return NotFound(new { message = "Documento no encontrado." });
        if (!System.IO.File.Exists(doc.RutaArchivo)) return NotFound(new { message = "El archivo físico no existe." });
        var bytes = await System.IO.File.ReadAllBytesAsync(doc.RutaArchivo, ct);
        return File(bytes, GetContentType(doc.Extension), doc.NombreArchivo);
    }

    [HttpDelete("documentos/{idDocumento:int}")]
    public async Task<IActionResult> EliminarDocumento(int idDocumento, CancellationToken ct)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni)) return Unauthorized(new { message = "Token sin claim 'dni'." });
        var res = await _service.EliminarDocumentoAsync(idDocumento, dni, ct);
        return res.Codigo == 1 ? Ok(res) : UnprocessableEntity(res);
    }

    private static NotificacionGuardarDto ToDto(NotificacionGuardarForm form) => new()
    {
        Id = form.Id,
        TipoDocumento = form.TipoDocumento,
        Asunto = form.Asunto ?? "",
        Mensaje = form.Mensaje ?? "",
        FechaEmision = form.FechaEmision,
        DniDestinatario = form.DniDestinatario ?? "",
        NombreDestinatario = form.NombreDestinatario,
        CargoDestinatario = form.CargoDestinatario,
        RegimenDestinatario = form.RegimenDestinatario
    };

    private async Task GuardarDocumentosAsync(int id, IFormFile? principal, List<IFormFile>? anexos, CancellationToken ct)
    {
        if (principal is not null && principal.Length > 0)
        {
            var saved = await SaveFileAsync(id, principal, "principal", ct);
            await _service.InsertarDocumentoAsync(id, "PRINCIPAL", principal.FileName, saved, Path.GetExtension(principal.FileName), principal.Length, ct);
        }
        if (anexos is not null)
        {
            foreach (var file in anexos.Where(x => x.Length > 0))
            {
                var saved = await SaveFileAsync(id, file, "anexos", ct);
                await _service.InsertarDocumentoAsync(id, "ANEXO", file.FileName, saved, Path.GetExtension(file.FileName), file.Length, ct);
            }
        }
    }

    private async Task GuardarDocumentosRespuestaAsync(int idRespuesta, IFormFile? principal, List<IFormFile>? anexos, CancellationToken ct)
    {
        if (principal is not null && principal.Length > 0)
        {
            var saved = await SaveFileAsync(idRespuesta, principal, "respuesta-principal", ct);
            await _service.InsertarRespuestaDocumentoAsync(idRespuesta, "PRINCIPAL", principal.FileName, saved, Path.GetExtension(principal.FileName), principal.Length, ct);
        }
        if (anexos is not null)
        {
            foreach (var file in anexos.Where(x => x.Length > 0))
            {
                var saved = await SaveFileAsync(idRespuesta, file, "respuesta-anexos", ct);
                await _service.InsertarRespuestaDocumentoAsync(idRespuesta, "ANEXO", file.FileName, saved, Path.GetExtension(file.FileName), file.Length, ct);
            }
        }
    }

    private async Task<string> SaveFileAsync(int id, IFormFile file, string folder, CancellationToken ct)
    {
        var baseRoot = ObtenerRutaBaseNotificaciones();

        // Estructura similar a FichaSalida, pero organizada por año, notificación/respuesta y tipo de archivo.
        // Ejemplo: F:\AGRONET\Uploads\Notificaciones\2026\1\principal\archivo.pdf
        var year = DateTime.Now.Year.ToString();
        var basePath = Path.Combine(baseRoot, year, id.ToString(), folder);

        Directory.CreateDirectory(basePath);

        var originalName = Path.GetFileName(file.FileName);
        var extension = Path.GetExtension(originalName);
        var finalName = $"{Guid.NewGuid():N}{extension}";
        var path = Path.Combine(basePath, finalName);

        await using var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await file.CopyToAsync(fs, ct);

        return path;
    }

    private string ObtenerRutaBaseNotificaciones()
    {
        var mode = (_configuration["NotificacionesStorage:Mode"] ?? "Local").Trim();
        var basePath = mode.Equals("UNC", StringComparison.OrdinalIgnoreCase)
            ? _configuration["NotificacionesStorage:UncBasePath"]
            : _configuration["NotificacionesStorage:LocalBasePath"];

        if (string.IsNullOrWhiteSpace(basePath))
            throw new InvalidOperationException("No se configuró NotificacionesStorage:LocalBasePath o NotificacionesStorage:UncBasePath.");

        return basePath;
    }

    private static string GetContentType(string? ext) => (ext ?? "").ToLowerInvariant() switch
    {
        ".pdf" => "application/pdf",
        ".doc" => "application/msword",
        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        ".xls" => "application/vnd.ms-excel",
        ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        _ => "application/octet-stream"
    };
}

public class NotificacionGuardarForm
{
    public int? Id { get; set; }
    public string? TipoDocumento { get; set; }
    public string? Asunto { get; set; }
    public string? Mensaje { get; set; }
    public DateTime? FechaEmision { get; set; }
    public string? DniDestinatario { get; set; }
    public string? NombreDestinatario { get; set; }
    public string? CargoDestinatario { get; set; }
    public string? RegimenDestinatario { get; set; }
    public IFormFile? DocumentoPrincipal { get; set; }
    public List<IFormFile>? Anexos { get; set; }
}

public class NotificacionRespuestaForm
{
    public string? Comentario { get; set; }
    public IFormFile? DocumentoPrincipal { get; set; }
    public List<IFormFile>? Anexos { get; set; }
}

public class NotificacionAnularRequest
{
    public string? Motivo { get; set; }
}
