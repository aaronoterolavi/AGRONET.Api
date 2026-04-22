using AGRONET.FichaSalida.Application.Contracts.FichaSalida;
using AGRONET.FichaSalida.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AGRONET.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class FichaSalidaController : ControllerBase
{
    private readonly IFichaSalidaService _svc;

    public FichaSalidaController(IFichaSalidaService svc)
    {
        _svc = svc;
    }

    [HttpPost("crear")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Crear([FromForm] FichaSalidaCrearForm form, CancellationToken ct)
    {
        var dni = User.FindFirstValue("dni");
        if (string.IsNullOrWhiteSpace(dni))
            return Unauthorized(new { message = "Token sin claim 'dni'." });

        // Map Form -> RequestDto (sin IFormFile)
        var req = new FichaSalidaCrearRequestDto
        {
            CodArea = form.CodArea,
            CodPer = form.CodPer,
            CodTipoEmpleado = form.CodTipoEmpleado,
            Destino = form.Destino,
            Motivo = form.Motivo,
            FechaInicio = form.FechaInicio,
            HoraInicio = form.HoraInicio,
            FechaFin = form.FechaFin,
            HoraFin = form.HoraFin,
            CodDestinoDetalle = form.CodDestinoDetalle
        };

        var res = await _svc.CrearAsync(dni, req, form.Documento, ct);

        if (res.IdFichaSalida is null)
            return UnprocessableEntity(res);

        return Ok(res);
    }

    // GET: /api/fichasalida/tipos
    [HttpGet("tipos")]
    public async Task<IActionResult> ListarTipos(CancellationToken ct)
    {
        var data = await _svc.ListarTiposAsync(ct);
        return Ok(data);
    }

    // GET: /api/fichasalida/tipos/{codigo}/detalles
    [HttpGet("tipos/{codigo}/detalles")]
    public async Task<IActionResult> ListarDetalles([FromRoute] string codigo, CancellationToken ct)
    {
        var data = await _svc.ListarDetallesPorTipoAsync(codigo, ct);
        return Ok(data);
    }

    // GET: /api/fichasalida/historial?estadoAutorizacion=APROBADA
    [HttpGet("historial")]
    public async Task<IActionResult> Historial(
      [FromQuery] string estadoAutorizacion,
      [FromQuery] int pageSize = 20,
      [FromQuery] int pageNumber = 0,
      CancellationToken ct = default)
    {
        var dni = User.FindFirstValue("dni");
        if (string.IsNullOrWhiteSpace(dni))
            return Unauthorized(new { message = "Token sin claim 'dni'." });

        var data = await _svc.ListarHistorialPorDniAsync(dni, estadoAutorizacion, pageSize, pageNumber, ct);
        return Ok(data);
    }

    [HttpGet("estados")]
    public async Task<IActionResult> ListarEstados(CancellationToken ct)
    {
        var data = await _svc.ListarEstadosAsync(ct);
        return Ok(data);
    }

    [Authorize]
    [HttpPut("anular")]
    public async Task<IActionResult> Anular([FromBody] FichaSalidaAnularRequestDto req, CancellationToken ct)
    {
        if (req is null || req.Id <= 0)
            return BadRequest(new { message = "Id inválido." });

        var res = await _svc.AnularAsync(req.Id, ct);

        if (res.Codigo != 1)
            return UnprocessableEntity(res);

        return Ok(res);
    }

    [Authorize]
    [HttpPut("actualizar-estado")]
    public async Task<IActionResult> ActualizarEstado([FromBody] FichaSalidaActualizarEstadoRequestDto req, CancellationToken ct)
    {
        if (req is null || req.Id <= 0)
            return BadRequest(new { message = "Id inválido." });

        if (string.IsNullOrWhiteSpace(req.EstadoAutorizacion))
            return BadRequest(new { message = "EstadoAutorizacion es requerido." });

        var res = await _svc.ActualizarEstadoAutorizacionAsync(req.Id, req.EstadoAutorizacion.Trim(), ct);

        if (res.Codigo != 1)
            return UnprocessableEntity(res);

        return Ok(res);
    }

    [Authorize]
    [HttpGet("autorizaciones")]
    public async Task<IActionResult> Listar(
            [FromQuery] string codArea,
            [FromQuery] string codTipoEmpleado,
            [FromQuery] string? estadoAutorizacion,
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
    {
        // DNI del autorizador desde JWT (tu claim "dni")
        var dni = User.FindFirstValue("dni");
        if (string.IsNullOrWhiteSpace(dni))
            return Unauthorized(new { message = "Token sin claim 'dni'." });

        var req = new FichaSalidaListarAutorizacionesRequestDto
        {
            EstadoAutorizacion = estadoAutorizacion,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _svc.ListarAsync(codArea, dni, codTipoEmpleado, req, ct);
        return Ok(result);
    }

    [HttpGet("listar-por-area-fechas")]
    public async Task<IActionResult> ListarPorAreaYFechas(
            [FromQuery] string codArea,
            [FromQuery] string estadoAutorizacion,
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fin,
            CancellationToken cancellationToken)
    {
        try
        {
            var request = new FichaSalidaListarPorAreaYFechasRequestDto
            {
                CodArea = codArea,
                EstadoAutorizacion = estadoAutorizacion,
                Inicio = inicio,
                Fin = fin
            };

            var result = await _svc.ListarPorAreaYFechasAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Ocurrió un error al listar las fichas de salida.",
                detail = ex.Message
            });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId([FromRoute] int id, CancellationToken ct)
    {
        if (id <= 0)
            return BadRequest(new { message = "Id inválido." });

        var result = await _svc.ObtenerPorIdAsync(id, ct);

        if (result is null)
            return NotFound(new { message = "No se encontró la ficha de salida." });

        return Ok(result);
    }

}
