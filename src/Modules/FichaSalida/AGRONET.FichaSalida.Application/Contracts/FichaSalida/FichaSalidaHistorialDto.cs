using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Contracts.FichaSalida
{
    public sealed class FichaSalidaHistorialDto
    {
        public int Id { get; set; }
        public DateTime? Fecha { get; set; }
        public string Usuario { get; set; } = default!;
        public string? Motivo { get; set; }

        public DateTime? FechaInicio { get; set; }
        public string? HoraInicio { get; set; }

        public DateTime? FechaFin { get; set; }
        public string? HoraFin { get; set; }

        public string? Destino { get; set; }
        public string? EstadoAutorizacion { get; set; }

        public string? FechaHoraInicio { get; set; }
        public string? FechaHoraFin { get; set; }
        public string? Justifica { get; set; }
        public string? StoragePath { get; set; }
    }
}
