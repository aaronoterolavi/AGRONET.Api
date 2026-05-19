using AGRONET.Bienes.Application.Contracts;
using AGRONET.Bienes.Application.DTOs.Bienes;
using AGRONET.Bienes.Application.DTOs.Common;
using AGRONET.Bienes.Application.DTOs.Licencias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AGRONET.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BienesController : ControllerBase
{
    private readonly IBienesService _bienesService;
    private readonly ILogger<BienesController> _logger;

    public BienesController(IBienesService bienesService, ILogger<BienesController> logger)
    {
        _bienesService = bienesService;
        _logger = logger;
    }

    // ========================= BIENES CRUD =========================

    /// <summary>
    /// Lista bienes tecnológicos con filtros y paginación
    /// </summary>
    [HttpGet("listar")]
    public async Task<IActionResult> ListarBienes(
        [FromQuery] string? codPatrimonial,
        [FromQuery] string? nombre,
        [FromQuery] int? tipoBienId,
        [FromQuery] int? oficinaId,
        [FromQuery] string? estadoFisico,
        [FromQuery] string? codArea,
        [FromQuery] int pageSize = 20,
        [FromQuery] int pageNumber = 0,
        CancellationToken ct = default)
    {
        try
        {
            var filtros = new BienListarFiltrosDto
            {
                cod_patrimonial = codPatrimonial,
                txt_nombre = nombre,
                ide_tipo_bien = tipoBienId,
                ide_oficina = oficinaId,
                est_fisico = estadoFisico,
                cod_area = codArea,
                page_size = pageSize,
                page_number = pageNumber
            };

            var result = await _bienesService.ListarBienesAsync(filtros, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar bienes");
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al listar los bienes"));
        }
    }

    /// <summary>
    /// Obtiene un bien por su ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerBienPorId([FromRoute] int id, CancellationToken ct = default)
    {
        try
        {
            if (id <= 0)
                return BadRequest(OperacionResultadoDto.Error("El ID es inválido"));

            var result = await _bienesService.ObtenerBienPorIdAsync(id, ct);

            if (result is null)
                return NotFound(OperacionResultadoDto.Error("No se encontró el bien"));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener bien por id {Id}", id);
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al obtener el bien"));
        }
    }

    /// <summary>
    /// Obtiene un bien por su código patrimonial
    /// </summary>
    [HttpGet("por-codigo/{codPatrimonial}")]
    public async Task<IActionResult> ObtenerBienPorCodPatrimonial([FromRoute] string codPatrimonial, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codPatrimonial))
                return BadRequest(OperacionResultadoDto.Error("El código patrimonial es inválido"));

            var result = await _bienesService.ObtenerBienPorCodPatrimonialAsync(codPatrimonial, ct);

            if (result is null)
                return NotFound(OperacionResultadoDto.Error("No se encontró el bien"));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener bien por código patrimonial {CodPatrimonial}", codPatrimonial);
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al obtener el bien"));
        }
    }

    /// <summary>
    /// Crea un nuevo bien tecnológico
    /// </summary>
    [HttpPost("crear")]
    public async Task<IActionResult> CrearBien([FromBody] BienCrearRequestDto request, CancellationToken ct = default)
    {
        try
        {
            var dni = User.FindFirstValue("dni");
            if (string.IsNullOrWhiteSpace(dni))
                return Unauthorized(OperacionResultadoDto.Error("Token sin claim 'dni'"));

            var result = await _bienesService.CrearBienAsync(dni, request, ct);

            if (result.codigo != 1)
                return UnprocessableEntity(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear bien");
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al crear el bien"));
        }
    }

    /// <summary>
    /// Actualiza un bien existente
    /// </summary>
    [HttpPut("actualizar")]
    public async Task<IActionResult> ActualizarBien([FromBody] BienActualizarRequestDto request, CancellationToken ct = default)
    {
        try
        {
            var dni = User.FindFirstValue("dni");
            if (string.IsNullOrWhiteSpace(dni))
                return Unauthorized(OperacionResultadoDto.Error("Token sin claim 'dni'"));

            if (request.ide_bien <= 0)
                return BadRequest(OperacionResultadoDto.Error("El ID del bien es inválido"));

            var result = await _bienesService.ActualizarBienAsync(dni, request, ct);

            if (result.codigo != 1)
                return UnprocessableEntity(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar bien {Id}", request.ide_bien);
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al actualizar el bien"));
        }
    }

    /// <summary>
    /// Elimina (soft delete) un bien
    /// </summary>
    [HttpPut("eliminar/{id:int}")]
    public async Task<IActionResult> EliminarBien([FromRoute] int id, CancellationToken ct = default)
    {
        try
        {
            if (id <= 0)
                return BadRequest(OperacionResultadoDto.Error("El ID es inválido"));

            var result = await _bienesService.EliminarBienAsync(id, ct);

            if (result.codigo != 1)
                return UnprocessableEntity(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar bien {Id}", id);
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al eliminar el bien"));
        }
    }

    // ========================= CATÁLOGOS =========================

    /// <summary>
    /// Lista los tipos de bien disponibles
    /// </summary>
    [HttpGet("catalogos/tipos-bien")]
    public async Task<IActionResult> ListarTiposBien(CancellationToken ct = default)
    {
        try
        {
            var result = await _bienesService.ListarTiposBienAsync(ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar tipos de bien");
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al listar los tipos de bien"));
        }
    }

    /// <summary>
    /// Lista las oficina
    /// </summary>
    [HttpGet("catalogos/oficinas")]
    public async Task<IActionResult> ListarOficinas(CancellationToken ct = default)
    {
        try
        {
            var result = await _bienesService.ListarOficinasAsync(ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar oficinas");
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al listar las oficinas"));
        }
    }

    /// <summary>
    /// Lista las marcas disponibles
    /// </summary>
    [HttpGet("catalogos/marcas")]
    public async Task<IActionResult> ListarMarcas(CancellationToken ct = default)
    {
        try
        {
            var result = await _bienesService.ListarMarcasAsync(ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar marcas");
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al listar las marcas"));
        }
    }

    /// <summary>
    /// Lista los procesadores disponibles
    /// </summary>
    [HttpGet("catalogos/procesadores")]
    public async Task<IActionResult> ListarProcesadores(CancellationToken ct = default)
    {
        try
        {
            var result = await _bienesService.ListarProcesadoresAsync(ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar procesadores");
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al listar los procesadores"));
        }
    }

    /// <summary>
    /// Lista el software disponible para licencias
    /// </summary>
    [HttpGet("catalogos/software")]
    public async Task<IActionResult> ListarSoftware(CancellationToken ct = default)
    {
        try
        {
            var result = await _bienesService.ListarSoftwareAsync(ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar software");
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al listar el software"));
        }
    }

    // ========================= LICENCIAS =========================

    /// <summary>
    /// Lista licencias de software con filtros y paginación
    /// </summary>
    [HttpGet("licencias/listar")]
    public async Task<IActionResult> ListarLicencias(
        [FromQuery] string? estadoLicencia,
        [FromQuery] int? softwareId,
        [FromQuery] int? diasVencer,
        [FromQuery] string? buscar,
        [FromQuery] int pageSize = 20,
        [FromQuery] int pageNumber = 0,
        CancellationToken ct = default)
    {
        try
        {
            var filtros = new LicenciaListarFiltrosDto
            {
                estado_licencia = estadoLicencia,
                ide_software = softwareId,
                dias_vencer = diasVencer,
                buscar = buscar,
                page_size = pageSize,
                page_number = pageNumber
            };

            var result = await _bienesService.ListarLicenciasAsync(filtros, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar licencias");
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al listar las licencias"));
        }
    }

    /// <summary>
    /// Obtiene una licencia por su ID
    /// </summary>
    [HttpGet("licencias/{id:int}")]
    public async Task<IActionResult> ObtenerLicenciaPorId([FromRoute] int id, CancellationToken ct = default)
    {
        try
        {
            if (id <= 0)
                return BadRequest(OperacionResultadoDto.Error("El ID es inválido"));

            var result = await _bienesService.ObtenerLicenciaPorIdAsync(id, ct);

            if (result is null)
                return NotFound(OperacionResultadoDto.Error("No se encontró la licencia"));

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener licencia por id {Id}", id);
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al obtener la licencia"));
        }
    }

    /// <summary>
    /// Crea una nueva licencia de software
    /// </summary>
    [HttpPost("licencias/crear")]
    public async Task<IActionResult> CrearLicencia([FromBody] LicenciaCrearRequestDto request, CancellationToken ct = default)
    {
        try
        {
            var dni = User.FindFirstValue("dni");
            if (string.IsNullOrWhiteSpace(dni))
                return Unauthorized(OperacionResultadoDto.Error("Token sin claim 'dni'"));

            var result = await _bienesService.CrearLicenciaAsync(dni, request, ct);

            if (result.codigo != 1)
                return UnprocessableEntity(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear licencia");
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al crear la licencia"));
        }
    }

    /// <summary>
    /// Elimina (soft delete) una licencia
    /// </summary>
    [HttpPut("licencias/eliminar/{id:int}")]
    public async Task<IActionResult> EliminarLicencia([FromRoute] int id, CancellationToken ct = default)
    {
        try
        {
            if (id <= 0)
                return BadRequest(OperacionResultadoDto.Error("El ID es inválido"));
            var result = await _bienesService.EliminarLicenciaAsync(id, ct);

            if (result.codigo != 1)
                return UnprocessableEntity(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar licencia {Id}", id);
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al eliminar la licencia"));
        }
    }

    // ========================= REPORTES =========================

    /// <summary>
    /// Reporte de licencias por vencer en X días
    /// </summary>
    [HttpGet("reportes/licencias-por-vencer")]
    public async Task<IActionResult> ReporteLicenciasPorVencer(
        [FromQuery] int dias = 30,
        CancellationToken ct = default)
    {
        try
        {
            if (dias <= 0)
                return BadRequest(OperacionResultadoDto.Error("El número de días debe ser mayor a cero"));

            var result = await _bienesService.ReporteLicenciasPorVencerAsync(dias, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de licencias por vencer");
            return StatusCode(500, OperacionResultadoDto.Error("Ocurrió un error al generar el reporte"));
        }
    }
}