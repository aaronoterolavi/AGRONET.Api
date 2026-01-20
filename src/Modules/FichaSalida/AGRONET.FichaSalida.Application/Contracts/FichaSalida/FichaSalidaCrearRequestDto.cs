using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Contracts.FichaSalida
{
    public sealed class FichaSalidaCrearRequestDto
    {
        public string CodArea { get; set; } = default!;
        public string CodPer { get; set; } = default!;
        public string CodTipoEmpleado { get; set; } = default!;
        public string Destino { get; set; } = default!;
        public string Motivo { get; set; } = default!;
        public DateOnly FechaInicio { get; set; }
        public string HoraInicio { get; set; } = default!;
        public DateOnly FechaFin { get; set; }
        public string HoraFin { get; set; } = default!;
        public string CodDestinoDetalle { get; set; } = default!;
    }
}
