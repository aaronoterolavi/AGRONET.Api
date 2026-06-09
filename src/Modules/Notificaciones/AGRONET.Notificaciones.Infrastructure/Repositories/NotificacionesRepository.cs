using System.Data;
using AGRONET.Notificaciones.Application.DTOs;
using AGRONET.Notificaciones.Application.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AGRONET.Notificaciones.Infrastructure.Repositories;

public class NotificacionesRepository : INotificacionesRepository
{
    private readonly string _connectionString;
    public NotificacionesRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("BD_AGRONET")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'BD_AGRONET'.");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<NotificacionCatalogosDto> ListarCatalogosAsync(CancellationToken ct = default)
    {
        using var con = CreateConnection();
        using var multi = await con.QueryMultipleAsync("dbo.USP_Notificaciones_ListarCatalogos", commandType: CommandType.StoredProcedure);
        return new NotificacionCatalogosDto
        {
            TiposDocumento = (await multi.ReadAsync<CatalogoItemDto>()).AsList(),
            EstadosRegistro = (await multi.ReadAsync<CatalogoItemDto>()).AsList(),
            EstadosCasilla = (await multi.ReadAsync<CatalogoItemDto>()).AsList()
        };
    }

    public async Task<IReadOnlyList<PersonalNotificacionDto>> BuscarPersonalAsync(string? texto, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        var rows = await con.QueryAsync<PersonalNotificacionDto>("dbo.USP_Notificaciones_BuscarPersonal", new { texto }, commandType: CommandType.StoredProcedure);
        return rows.AsList();
    }

    public async Task<IReadOnlyList<NotificacionListadoDto>> ListarRegistroAsync(string dniUsuario, string? estado, string? tipoDocumento, string? buscar, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        using var multi = await con.QueryMultipleAsync("dbo.USP_Notificaciones_ListarRegistro", new { vDniUsuario = dniUsuario, estado, tipoDocumento, buscar }, commandType: CommandType.StoredProcedure);
        var rows = (await multi.ReadAsync<NotificacionFlat>()).AsList();
        var docs = (await multi.ReadAsync<DocumentoFlat>()).AsList();
        var resps = (await multi.ReadAsync<RespuestaFlat>()).AsList();
        return MapListado(rows, docs, resps).AsReadOnly();
    }

    public async Task<IReadOnlyList<NotificacionCasillaDto>> ListarCasillaAsync(string dni, string? estado, string? buscar, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        using var multi = await con.QueryMultipleAsync("dbo.USP_Notificaciones_CasillaPorDni", new { vDni = dni, estado, buscar }, commandType: CommandType.StoredProcedure);
        var rows = (await multi.ReadAsync<CasillaFlat>()).AsList();
        var docs = (await multi.ReadAsync<DocumentoFlat>()).AsList();
        var resps = (await multi.ReadAsync<RespuestaFlat>()).AsList();
        return MapCasilla(rows, docs, resps).AsReadOnly();
    }

    public async Task<IReadOnlyList<NotificacionReporteDto>> ReporteAsync(DateTime? fechaInicio, DateTime? fechaFin, string? estado, string? buscar, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        var rows = await con.QueryAsync<ReporteFlat>("dbo.USP_Notificaciones_Reporte", new { fechaInicio, fechaFin, estado, buscar }, commandType: CommandType.StoredProcedure);
        return rows.Select(x => new NotificacionReporteDto
        {
            Id = x.Id,
            Destinatario = new PersonalNotificacionDto { Id = x.VDniDestinatario, Dni = x.VDniDestinatario, NombreCompleto = x.NombreDestinatario ?? "", Cargo = x.CargoDestinatario },
            Asunto = x.Asunto ?? "",
            NumeroDocumento = x.NumeroDocumento,
            FechaDocumento = x.FechaDocumento,
            FechaEnvio = x.FechaEnvio,
            Remitente = x.Remitente ?? "",
            Recibido = x.Recibido,
            FechaAcuse = x.FechaAcuse,
            Respuesta = x.TieneRespuesta ? new NotificacionRespuestaDto { Id = x.IdRespuesta ?? 0, Fecha = x.FechaRespuesta, DocumentoPrincipal = string.IsNullOrWhiteSpace(x.DocumentoRespuesta) ? null : new NotificacionDocumentoDto { Nombre = x.DocumentoRespuesta }, Comentario = x.ComentarioRespuesta } : null
        }).ToList().AsReadOnly();
    }

    public async Task<NotificacionListadoDto?> ObtenerDetalleAsync(int id, string dniUsuario, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        using var multi = await con.QueryMultipleAsync("dbo.USP_Notificaciones_ObtenerDetalle", new { iCodNotificacion = id, vDniUsuario = dniUsuario }, commandType: CommandType.StoredProcedure);
        var rows = (await multi.ReadAsync<NotificacionFlat>()).AsList();
        var docs = (await multi.ReadAsync<DocumentoFlat>()).AsList();
        var resps = (await multi.ReadAsync<RespuestaFlat>()).AsList();
        return MapListado(rows, docs, resps).FirstOrDefault();
    }

    public Task<OperacionResultadoDto> GuardarBorradorAsync(string dniUsuario, NotificacionGuardarDto dto, CancellationToken ct = default)
        => GuardarAsync("dbo.USP_Notificaciones_GuardarBorrador", dniUsuario, dto);
    public Task<OperacionResultadoDto> PublicarAsync(string dniUsuario, NotificacionGuardarDto dto, CancellationToken ct = default)
        => GuardarAsync("dbo.USP_Notificaciones_Publicar", dniUsuario, dto);

    private async Task<OperacionResultadoDto> GuardarAsync(string sp, string dniUsuario, NotificacionGuardarDto dto)
    {
        using var con = CreateConnection();
        var p = new DynamicParameters();
        p.Add("@iCodNotificacion", dto.Id, DbType.Int32);
        p.Add("@vTipoDocumento", dto.TipoDocumento, DbType.String);
        p.Add("@vAsunto", dto.Asunto, DbType.String);
        p.Add("@vMensaje", dto.Mensaje, DbType.String);
        p.Add("@dtFechaEmision", dto.FechaEmision, DbType.Date);
        p.Add("@vDniDestinatario", dto.DniDestinatario, DbType.String);
        p.Add("@vNombreDestinatario", dto.NombreDestinatario, DbType.String);
        p.Add("@vCargoDestinatario", dto.CargoDestinatario, DbType.String);
        p.Add("@vRegimenDestinatario", dto.RegimenDestinatario, DbType.String);
        p.Add("@vDniUsuarioRegistro", dniUsuario, DbType.String);
        return await con.QueryFirstAsync<OperacionResultadoDto>(sp, p, commandType: CommandType.StoredProcedure);
    }

    public async Task<OperacionResultadoDto> AnularAsync(int id, string dniUsuario, string motivo, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        return await con.QueryFirstAsync<OperacionResultadoDto>("dbo.USP_Notificaciones_Anular", new { iCodNotificacion = id, vDniUsuarioAnulacion = dniUsuario, vMotivoAnulacion = motivo }, commandType: CommandType.StoredProcedure);
    }

    public async Task<OperacionResultadoDto> AceptarAcuseAsync(int id, string dniUsuario, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        return await con.QueryFirstAsync<OperacionResultadoDto>("dbo.USP_Notificaciones_AceptarAcuse", new { iCodNotificacion = id, vDni = dniUsuario }, commandType: CommandType.StoredProcedure);
    }

    public async Task<OperacionResultadoDto> RegistrarRespuestaAsync(int id, string dniUsuario, NotificacionRespuestaGuardarDto dto, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        return await con.QueryFirstAsync<OperacionResultadoDto>("dbo.USP_Notificaciones_RegistrarRespuesta", new { iCodNotificacion = id, vDniRespuesta = dniUsuario, vComentario = dto.Comentario }, commandType: CommandType.StoredProcedure);
    }

    public async Task<OperacionResultadoDto> InsertarDocumentoAsync(int id, string tipoArchivo, string nombreArchivo, string rutaArchivo, string? extension, long tamanioBytes, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        return await con.QueryFirstAsync<OperacionResultadoDto>("dbo.USP_Notificaciones_Documento_Insertar", new { iCodNotificacion = id, vTipoArchivo = tipoArchivo, vNombreArchivo = nombreArchivo, vRutaArchivo = rutaArchivo, vExtension = extension, iTamanioBytes = tamanioBytes }, commandType: CommandType.StoredProcedure);
    }

    public async Task<OperacionResultadoDto> InsertarRespuestaDocumentoAsync(int idRespuesta, string tipoArchivo, string nombreArchivo, string rutaArchivo, string? extension, long tamanioBytes, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        return await con.QueryFirstAsync<OperacionResultadoDto>("dbo.USP_Notificaciones_RespuestaDocumento_Insertar", new { iCodRespuesta = idRespuesta, vTipoArchivo = tipoArchivo, vNombreArchivo = nombreArchivo, vRutaArchivo = rutaArchivo, vExtension = extension, iTamanioBytes = tamanioBytes }, commandType: CommandType.StoredProcedure);
    }

    public async Task<NotificacionDocumentoRegistroDto?> ObtenerDocumentoAsync(int idDocumento, string dniUsuario, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        return await con.QueryFirstOrDefaultAsync<NotificacionDocumentoRegistroDto>("dbo.USP_Notificaciones_Documento_Obtener", new { iCodDocumento = idDocumento, vDniUsuario = dniUsuario }, commandType: CommandType.StoredProcedure);
    }

    public async Task<OperacionResultadoDto> EliminarDocumentoAsync(int idDocumento, string dniUsuario, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        return await con.QueryFirstAsync<OperacionResultadoDto>("dbo.USP_Notificaciones_Documento_Eliminar", new { iCodDocumento = idDocumento, vDniUsuario = dniUsuario }, commandType: CommandType.StoredProcedure);
    }

    public async Task<int?> ObtenerIdRespuestaAsync(int idNotificacion, string dniUsuario, CancellationToken ct = default)
    {
        using var con = CreateConnection();
        return await con.ExecuteScalarAsync<int?>("dbo.USP_Notificaciones_Respuesta_ObtenerId", new { iCodNotificacion = idNotificacion, vDniRespuesta = dniUsuario }, commandType: CommandType.StoredProcedure);
    }

    private static List<NotificacionListadoDto> MapListado(List<NotificacionFlat> rows, List<DocumentoFlat> docs, List<RespuestaFlat> resps)
    {
        return rows.Select(x => new NotificacionListadoDto
        {
            Id = x.Id, Codigo = x.Codigo ?? "", TipoDocumento = x.TipoDocumento,
            Asunto = x.Asunto ?? "", Mensaje = x.Mensaje ?? "",
            FechaEmision = x.FechaEmision, FechaPublicacion = x.FechaPublicacion,
            Destinatario = new PersonalNotificacionDto { Id = x.VDniDestinatario ?? "", Dni = x.VDniDestinatario ?? "", NombreCompleto = x.NombreDestinatario ?? "", Cargo = x.CargoDestinatario, Regimen = x.RegimenDestinatario },
            Remitente = new PersonalNotificacionDto { Id = x.VDniUsuarioRegistro ?? "", Dni = x.VDniUsuarioRegistro ?? "", NombreCompleto = x.NombreRemitente ?? "", Cargo = x.CargoRemitente },
            Estado = x.Estado ?? "", Visualizada = x.Visualizada, FechaVisualizacion = x.FechaVisualizacion, AcuseAceptado = x.AcuseAceptado, FechaAcuse = x.FechaAcuse, PuedeAnular = !x.Visualizada && x.Estado == "PUBLICADA",
            DocumentoPrincipal = docs.Where(d=>d.ICodNotificacion==x.Id && d.TipoArchivo=="PRINCIPAL" && !d.EsRespuesta).Select(MapDoc).FirstOrDefault(),
            Anexos = docs.Where(d=>d.ICodNotificacion==x.Id && d.TipoArchivo=="ANEXO" && !d.EsRespuesta).Select(MapDoc).ToList(),
            Respuesta = MapRespuesta(resps.FirstOrDefault(r=>r.ICodNotificacion==x.Id), docs.Where(d=>d.ICodNotificacion==x.Id && d.EsRespuesta).ToList())
        }).ToList();
    }

    private static List<NotificacionCasillaDto> MapCasilla(List<CasillaFlat> rows, List<DocumentoFlat> docs, List<RespuestaFlat> resps)
    {
        return rows.Select(x => new NotificacionCasillaDto
        {
            Id = x.Id, Remitente = x.Remitente ?? "", CargoRemitente = x.CargoRemitente,
            Asunto = x.Asunto ?? "", Mensaje = x.Mensaje ?? "", NumeroDocumento = x.NumeroDocumento,
            FechaDocumento = x.FechaDocumento, FechaEnvio = x.FechaEnvio,
            Recibido = x.Recibido, FechaAcuse = x.FechaAcuse, Leido = x.Leido,
            Documentos = new NotificacionDocumentosCasillaDto
            {
                Principal = docs.Where(d=>d.ICodNotificacion==x.Id && d.TipoArchivo=="PRINCIPAL" && !d.EsRespuesta).Select(MapDoc).FirstOrDefault(),
                Anexos = docs.Where(d=>d.ICodNotificacion==x.Id && d.TipoArchivo=="ANEXO" && !d.EsRespuesta).Select(MapDoc).ToList()
            },
            Respuesta = MapRespuesta(resps.FirstOrDefault(r=>r.ICodNotificacion==x.Id), docs.Where(d=>d.ICodNotificacion==x.Id && d.EsRespuesta).ToList())
        }).ToList();
    }

    private static NotificacionDocumentoDto MapDoc(DocumentoFlat d)
    {
        // Los documentos de respuesta usan el mismo identity que otra tabla.
        // Para evitar colisiones en descarga, se envían con ID negativo y el SP los resuelve con ABS(id).
        var idDescarga = d.EsRespuesta ? -d.Id : d.Id;
        return new NotificacionDocumentoDto
        {
            Id = idDescarga,
            Nombre = d.Nombre ?? "",
            Tamaño = d.Tamaño,
            Tipo = d.Tipo,
            Url = $"/Notificaciones/DescargarDocumento?idDocumento={idDescarga}",
            Descargado = d.Descargado,
            FechaDescarga = d.FechaDescarga
        };
    }

    private static NotificacionRespuestaDto? MapRespuesta(RespuestaFlat? r, List<DocumentoFlat> docs)
    {
        if (r is null) return null;
        return new NotificacionRespuestaDto
        {
            Id = r.Id, Fecha = r.Fecha, Comentario = r.Comentario,
            DocumentoPrincipal = docs.Where(d=>d.TipoArchivo=="PRINCIPAL").Select(MapDoc).FirstOrDefault(),
            Anexos = docs.Where(d=>d.TipoArchivo=="ANEXO").Select(MapDoc).ToList()
        };
    }

    private sealed class NotificacionFlat { public int Id {get;set;} public string? Codigo{get;set;} public string? TipoDocumento{get;set;} public string? Asunto{get;set;} public string? Mensaje{get;set;} public DateTime? FechaEmision{get;set;} public DateTime? FechaPublicacion{get;set;} public string? VDniDestinatario{get;set;} public string? NombreDestinatario{get;set;} public string? CargoDestinatario{get;set;} public string? RegimenDestinatario{get;set;} public string? VDniUsuarioRegistro{get;set;} public string? NombreRemitente{get;set;} public string? CargoRemitente{get;set;} public string? Estado{get;set;} public bool Visualizada{get;set;} public DateTime? FechaVisualizacion{get;set;} public bool AcuseAceptado{get;set;} public DateTime? FechaAcuse{get;set;} }
    private sealed class CasillaFlat { public int Id{get;set;} public string? Remitente{get;set;} public string? CargoRemitente{get;set;} public string? Asunto{get;set;} public string? Mensaje{get;set;} public string? NumeroDocumento{get;set;} public DateTime? FechaDocumento{get;set;} public DateTime? FechaEnvio{get;set;} public bool Recibido{get;set;} public DateTime? FechaAcuse{get;set;} public bool Leido{get;set;} }
    private sealed class DocumentoFlat { public int Id{get;set;} public int ICodNotificacion{get;set;} public string? TipoArchivo{get;set;} public string? Nombre{get;set;} public long Tamaño{get;set;} public string? Tipo{get;set;} public bool Descargado{get;set;} public DateTime? FechaDescarga{get;set;} public bool EsRespuesta{get;set;} }
    private sealed class RespuestaFlat { public int Id{get;set;} public int ICodNotificacion{get;set;} public DateTime? Fecha{get;set;} public string? Comentario{get;set;} }
    private sealed class ReporteFlat { public int Id{get;set;} public string? VDniDestinatario{get;set;} public string? NombreDestinatario{get;set;} public string? CargoDestinatario{get;set;} public string? Asunto{get;set;} public string? NumeroDocumento{get;set;} public DateTime? FechaDocumento{get;set;} public DateTime? FechaEnvio{get;set;} public string? Remitente{get;set;} public bool Recibido{get;set;} public DateTime? FechaAcuse{get;set;} public bool TieneRespuesta{get;set;} public int? IdRespuesta{get;set;} public DateTime? FechaRespuesta{get;set;} public string? DocumentoRespuesta{get;set;} public string? ComentarioRespuesta{get;set;} }
}
