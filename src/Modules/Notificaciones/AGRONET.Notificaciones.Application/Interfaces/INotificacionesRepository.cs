using AGRONET.Notificaciones.Application.DTOs;

namespace AGRONET.Notificaciones.Application.Interfaces;

public interface INotificacionesRepository
{
    Task<NotificacionCatalogosDto> ListarCatalogosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<PersonalNotificacionDto>> BuscarPersonalAsync(string? texto, CancellationToken ct = default);
    Task<IReadOnlyList<NotificacionListadoDto>> ListarRegistroAsync(string dniUsuario, string? estado, string? tipoDocumento, string? buscar, CancellationToken ct = default);
    Task<IReadOnlyList<NotificacionCasillaDto>> ListarCasillaAsync(string dni, string? estado, string? buscar, CancellationToken ct = default);
    Task<IReadOnlyList<NotificacionReporteDto>> ReporteAsync(DateTime? fechaInicio, DateTime? fechaFin, string? estado, string? buscar, CancellationToken ct = default);
    Task<NotificacionListadoDto?> ObtenerDetalleAsync(int id, string dniUsuario, CancellationToken ct = default);
    Task<OperacionResultadoDto> GuardarBorradorAsync(string dniUsuario, NotificacionGuardarDto dto, CancellationToken ct = default);
    Task<OperacionResultadoDto> PublicarAsync(string dniUsuario, NotificacionGuardarDto dto, CancellationToken ct = default);
    Task<OperacionResultadoDto> AnularAsync(int id, string dniUsuario, string motivo, CancellationToken ct = default);
    Task<OperacionResultadoDto> AceptarAcuseAsync(int id, string dniUsuario, CancellationToken ct = default);
    Task<OperacionResultadoDto> RegistrarRespuestaAsync(int id, string dniUsuario, NotificacionRespuestaGuardarDto dto, CancellationToken ct = default);
    Task<OperacionResultadoDto> InsertarDocumentoAsync(int id, string tipoArchivo, string nombreArchivo, string rutaArchivo, string? extension, long tamanioBytes, CancellationToken ct = default);
    Task<OperacionResultadoDto> InsertarRespuestaDocumentoAsync(int idRespuesta, string tipoArchivo, string nombreArchivo, string rutaArchivo, string? extension, long tamanioBytes, CancellationToken ct = default);
    Task<NotificacionDocumentoRegistroDto?> ObtenerDocumentoAsync(int idDocumento, string dniUsuario, CancellationToken ct = default);
    Task<OperacionResultadoDto> EliminarDocumentoAsync(int idDocumento, string dniUsuario, CancellationToken ct = default);
    Task<int?> ObtenerIdRespuestaAsync(int idNotificacion, string dniUsuario, CancellationToken ct = default);
}
