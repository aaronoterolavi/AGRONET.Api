using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Contracts.FichaSalida
{
    public sealed class FichaSalidaAutorizacionDto
    {
        public string? EstadoAutorizacion { get; set; }
        public string? DNI { get; set; }
        public string? HoraFin { get; set; }
        public string? HoraInicio { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int Id { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Destino { get; set; }
        public string? Motivo { get; set; }
        public string? Usuario { get; set; }
        public string? IngresoReal { get; set; }
        public string? SalidaReal { get; set; }
        public string? ObservacionesVigilancia { get; set; }
        public string? Nombre { get; set; }
        public string? AREA { get; set; }
        public string? TipoPermiso { get; set; }
        public string? Codigo { get; set; }
        public string? cod_area { get; set; }
        public string? cod_tipo_empleado { get; set; }
        public string? StoragePath { get; set; }
    }
}
