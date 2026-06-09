using AGRONET.Notificaciones.Application.DTOs;
using AGRONET.Notificaciones.Application.Interfaces;

namespace AGRONET.Notificaciones.Application.Services;

public class NotificacionesService : INotificacionesService
{
    private readonly INotificacionesRepository _repo;
    public NotificacionesService(INotificacionesRepository repo) => _repo = repo;

    public Task<NotificacionCatalogosDto> ListarCatalogosAsync(CancellationToken ct = default) => _repo.ListarCatalogosAsync(ct);
    public Task<IReadOnlyList<PersonalNotificacionDto>> BuscarPersonalAsync(string? texto, CancellationToken ct = default) => _repo.BuscarPersonalAsync(texto, ct);
    public Task<IReadOnlyList<NotificacionListadoDto>> ListarRegistroAsync(string dniUsuario, string? estado, string? tipoDocumento, string? buscar, CancellationToken ct = default) => _repo.ListarRegistroAsync(dniUsuario, estado, tipoDocumento, buscar, ct);
    public Task<IReadOnlyList<NotificacionCasillaDto>> ListarCasillaAsync(string dni, string? estado, string? buscar, CancellationToken ct = default) => _repo.ListarCasillaAsync(dni, estado, buscar, ct);
    public Task<IReadOnlyList<NotificacionReporteDto>> ReporteAsync(DateTime? fechaInicio, DateTime? fechaFin, string? estado, string? buscar, CancellationToken ct = default) => _repo.ReporteAsync(fechaInicio, fechaFin, estado, buscar, ct);
    public Task<NotificacionListadoDto?> ObtenerDetalleAsync(int id, string dniUsuario, CancellationToken ct = default) => _repo.ObtenerDetalleAsync(id, dniUsuario, ct);
    public Task<OperacionResultadoDto> GuardarBorradorAsync(string dniUsuario, NotificacionGuardarDto dto, CancellationToken ct = default) => _repo.GuardarBorradorAsync(dniUsuario, dto, ct);
    public Task<OperacionResultadoDto> PublicarAsync(string dniUsuario, NotificacionGuardarDto dto, CancellationToken ct = default) => _repo.PublicarAsync(dniUsuario, dto, ct);
    public Task<OperacionResultadoDto> AnularAsync(int id, string dniUsuario, string motivo, CancellationToken ct = default) => _repo.AnularAsync(id, dniUsuario, motivo, ct);
    public Task<OperacionResultadoDto> AceptarAcuseAsync(int id, string dniUsuario, CancellationToken ct = default) => _repo.AceptarAcuseAsync(id, dniUsuario, ct);
    public Task<OperacionResultadoDto> RegistrarRespuestaAsync(int id, string dniUsuario, NotificacionRespuestaGuardarDto dto, CancellationToken ct = default) => _repo.RegistrarRespuestaAsync(id, dniUsuario, dto, ct);
    public Task<OperacionResultadoDto> InsertarDocumentoAsync(int id, string tipoArchivo, string nombreArchivo, string rutaArchivo, string? extension, long tamanioBytes, CancellationToken ct = default) => _repo.InsertarDocumentoAsync(id, tipoArchivo, nombreArchivo, rutaArchivo, extension, tamanioBytes, ct);
    public Task<OperacionResultadoDto> InsertarRespuestaDocumentoAsync(int idRespuesta, string tipoArchivo, string nombreArchivo, string rutaArchivo, string? extension, long tamanioBytes, CancellationToken ct = default) => _repo.InsertarRespuestaDocumentoAsync(idRespuesta, tipoArchivo, nombreArchivo, rutaArchivo, extension, tamanioBytes, ct);
    public Task<NotificacionDocumentoRegistroDto?> ObtenerDocumentoAsync(int idDocumento, string dniUsuario, CancellationToken ct = default) => _repo.ObtenerDocumentoAsync(idDocumento, dniUsuario, ct);
    public Task<OperacionResultadoDto> EliminarDocumentoAsync(int idDocumento, string dniUsuario, CancellationToken ct = default) => _repo.EliminarDocumentoAsync(idDocumento, dniUsuario, ct);
    public Task<int?> ObtenerIdRespuestaAsync(int idNotificacion, string dniUsuario, CancellationToken ct = default) => _repo.ObtenerIdRespuestaAsync(idNotificacion, dniUsuario, ct);
}
