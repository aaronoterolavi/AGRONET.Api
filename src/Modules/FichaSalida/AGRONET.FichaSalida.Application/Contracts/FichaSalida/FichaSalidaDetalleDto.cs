namespace AGRONET.FichaSalida.Application.Contracts.FichaSalida;

public class FichaSalidaDetalleDto
{
    public int Id { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Usuario { get; set; }
    public string? CodArea { get; set; }
    public string? CodTipoEmpleado { get; set; }
    public string? Destino { get; set; }
    public string? CodDestinoDetalle { get; set; }
    public string? Motivo { get; set; }
    public DateTime? FechaInicio { get; set; }
    public string? HoraInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? HoraFin { get; set; }
    public string? EstadoAutorizacion { get; set; }
    public string? Estado { get; set; }
    public DateTime? SalidaReal { get; set; }
    public DateTime? IngresoReal { get; set; }
    public string? ObservacionesVigilancia { get; set; }
    public string? DocumentosEssalud { get; set; }
    public string? CodAutorizacionVigilante { get; set; }
    public string? IpAutorizacion { get; set; }
    public DateTime? FechaAprueba { get; set; }
    public DateTime? FechaDenegado { get; set; }
    public string? AprobadoDenegado { get; set; }
}