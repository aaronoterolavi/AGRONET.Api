using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Marcacion.Application.Contracts
{
    public sealed class ReporteMarcacionDto
    {
        public string CodArea { get; set; } = "";
        public DateTime FecMarca { get; set; }
        public string Dni { get; set; } = "";
        public string Nombre { get; set; } = "";


        public TimeSpan? Ingreso { get; set; }
        public TimeSpan? Almuerzo { get; set; }
        public TimeSpan? Regreso { get; set; }
        public TimeSpan? Salida { get; set; }

        public DateTime DtFAuditoria { get; set; }

        // (opcional) propiedades “para mostrar”
        public string IngresoTxt => Ingreso?.ToString(@"hh\:mm") ?? "00:00";
        public string AlmuerzoTxt => Almuerzo?.ToString(@"hh\:mm") ?? "00:00";
        public string RegresoTxt => Regreso?.ToString(@"hh\:mm") ?? "00:00";
        public string SalidaTxt => Salida?.ToString(@"hh\:mm") ?? "00:00";
        public string ObsPapeleta { get; set; }
    }
}
