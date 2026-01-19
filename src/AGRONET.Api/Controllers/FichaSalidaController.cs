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
    public async Task<IActionResult> Historial([FromQuery] string estadoAutorizacion, CancellationToken ct)
    {
        // ✅ usuario sale del token (no lo recibe por query)
        //  var username = User.FindFirstValue("username");
        var dni = User.FindFirstValue("dni");
        if (string.IsNullOrWhiteSpace(dni))
            return Unauthorized(new { message = "Token sin claim 'dni'." });

        var data = await _svc.ListarHistorialPorDniAsync(dni, estadoAutorizacion, ct);
        return Ok(data);
    }
}
