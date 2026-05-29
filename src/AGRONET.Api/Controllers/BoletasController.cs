using AGRONET.Boletas.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGRONET.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BoletasController : ControllerBase
{
    private readonly IBoletasService _service;

    public BoletasController(IBoletasService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string anio, [FromQuery] string mes)
    {
        if (string.IsNullOrWhiteSpace(anio))
            return BadRequest("El parámetro 'anio' es obligatorio.");

        if (string.IsNullOrWhiteSpace(mes))
            return BadRequest("El parámetro 'mes' es obligatorio.");

        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni))
            return Unauthorized("No se encontró el DNI en el token.");

        var result = await _service.ListarPorDniAnioMesAsync(dni, anio, mes);
        return Ok(result);
    }

    [HttpGet("{iCodBoleta:int}/archivo")]
    public async Task<IActionResult> ObtenerArchivo(int iCodBoleta)
    {
        var dni = ObtenerDniDesdeToken();
        if (string.IsNullOrWhiteSpace(dni))
            return Unauthorized("No se encontró el DNI en el token.");

        var boleta = await _service.ObtenerPorIdYDniAsync(iCodBoleta, dni);

        if (boleta is null)
            return NotFound("No se encontró la boleta para el usuario autenticado.");

        if (string.IsNullOrWhiteSpace(boleta.VRutaBoleta))
            return NotFound("La boleta no tiene una ruta de archivo válida.");

        if (!System.IO.File.Exists(boleta.VRutaBoleta))
            return NotFound("El archivo físico de la boleta no existe.");

        await _service.MarcarVistoAsync(iCodBoleta, dni);
        await _service.MarcarDescargadoAsync(iCodBoleta, dni);

        var fileBytes = await System.IO.File.ReadAllBytesAsync(boleta.VRutaBoleta);
        var fileName = Path.GetFileName(boleta.VRutaBoleta);

        return File(fileBytes, "application/pdf", fileName);
    }

    private string? ObtenerDniDesdeToken()
    {
        return User.FindFirst("dni")?.Value;
    }

    [HttpGet("planilla-resumen")]
    public async Task<IActionResult> ObtenerPlanillaResumen([FromQuery] string periodo)
    {
        if (string.IsNullOrWhiteSpace(periodo))
            return BadRequest("El parámetro 'periodo' es obligatorio. Ejemplo: 2026-05.");

        periodo = periodo.Trim();

        if (periodo.Length != 7 || periodo[4] != '-')
            return BadRequest("El parámetro 'periodo' debe tener el formato yyyy-MM. Ejemplo: 2026-05.");

        var anio = periodo.Substring(0, 4);
        var mes = periodo.Substring(5, 2);

        if (!int.TryParse(anio, out _))
            return BadRequest("El año del periodo no es válido.");

        if (!int.TryParse(mes, out var mesNumero) || mesNumero < 1 || mesNumero > 12)
            return BadRequest("El mes del periodo no es válido.");

        var dni = ObtenerDniDesdeToken();

        if (string.IsNullOrWhiteSpace(dni))
            return Unauthorized("No se encontró el DNI en el token.");

        var result = await _service.ObtenerPlanillaResumenAsync(dni, periodo);

        return Ok(result);
    }

    
}