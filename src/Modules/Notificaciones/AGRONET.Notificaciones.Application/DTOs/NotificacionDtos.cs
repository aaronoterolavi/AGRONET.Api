namespace AGRONET.Notificaciones.Application.DTOs;

public class OperacionResultadoDto
{
    public int Codigo { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int? Id { get; set; }
}

public class CatalogoItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

public class NotificacionCatalogosDto
{
    public IEnumerable<CatalogoItemDto> TiposDocumento { get; set; } = [];
    public IEnumerable<CatalogoItemDto> EstadosRegistro { get; set; } = [];
    public IEnumerable<CatalogoItemDto> EstadosCasilla { get; set; } = [];
}

public class PersonalNotificacionDto
{
    public string Id { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Regimen { get; set; }
    public string? Cargo { get; set; }
}

public class NotificacionGuardarDto
{
    public int? Id { get; set; }
    public string? TipoDocumento { get; set; }
    public string Asunto { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime? FechaEmision { get; set; }
    public string DniDestinatario { get; set; } = string.Empty;
    public string? NombreDestinatario { get; set; }
    public string? CargoDestinatario { get; set; }
    public string? RegimenDestinatario { get; set; }
}

public class NotificacionDocumentoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public long Tamaño { get; set; }
    public string? Tipo { get; set; }
    public string? Url { get; set; }
    public bool Descargado { get; set; }
    public DateTime? FechaDescarga { get; set; }
}

public class NotificacionDocumentoRegistroDto
{
    public int Id { get; set; }
    public int ICodNotificacion { get; set; }
    public string TipoArchivo { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public string? Extension { get; set; }
    public long TamanioBytes { get; set; }
}

public class NotificacionListadoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string? TipoDocumento { get; set; }
    public string Asunto { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime? FechaEmision { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public PersonalNotificacionDto Destinatario { get; set; } = new();
    public PersonalNotificacionDto Remitente { get; set; } = new();
    public string Estado { get; set; } = string.Empty;
    public bool Visualizada { get; set; }
    public DateTime? FechaVisualizacion { get; set; }
    public bool AcuseAceptado { get; set; }
    public DateTime? FechaAcuse { get; set; }
    public bool PuedeAnular { get; set; }
    public NotificacionDocumentoDto? DocumentoPrincipal { get; set; }
    public IEnumerable<NotificacionDocumentoDto> Anexos { get; set; } = [];
    public NotificacionRespuestaDto? Respuesta { get; set; }
}

public class NotificacionDocumentosCasillaDto
{
    public NotificacionDocumentoDto? Principal { get; set; }
    public IEnumerable<NotificacionDocumentoDto> Anexos { get; set; } = [];
}

public class NotificacionCasillaDto
{
    public int Id { get; set; }
    public string Remitente { get; set; } = string.Empty;
    public string? CargoRemitente { get; set; }
    public string Asunto { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string? NumeroDocumento { get; set; }
    public DateTime? FechaDocumento { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public NotificacionDocumentosCasillaDto Documentos { get; set; } = new();
    public bool Recibido { get; set; }
    public DateTime? FechaAcuse { get; set; }
    public bool Leido { get; set; }
    public NotificacionRespuestaDto? Respuesta { get; set; }
}

public class NotificacionRespuestaDto
{
    public int Id { get; set; }
    public DateTime? Fecha { get; set; }
    public NotificacionDocumentoDto? DocumentoPrincipal { get; set; }
    public IEnumerable<NotificacionDocumentoDto> Anexos { get; set; } = [];
    public string? Comentario { get; set; }
}

public class NotificacionRespuestaGuardarDto
{
    public string? Comentario { get; set; }
}

public class NotificacionReporteDto
{
    public int Id { get; set; }
    public PersonalNotificacionDto Destinatario { get; set; } = new();
    public string Asunto { get; set; } = string.Empty;
    public string? NumeroDocumento { get; set; }
    public DateTime? FechaDocumento { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public string Remitente { get; set; } = string.Empty;
    public bool Recibido { get; set; }
    public DateTime? FechaAcuse { get; set; }
    public NotificacionRespuestaDto? Respuesta { get; set; }
}
