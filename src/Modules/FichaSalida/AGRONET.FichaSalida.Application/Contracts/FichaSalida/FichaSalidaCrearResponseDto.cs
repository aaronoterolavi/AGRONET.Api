using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Contracts.FichaSalida
{
    public sealed class FichaSalidaCrearResponseDto
    {
        public int? IdFichaSalida { get; set; }
        public string MensajeSalida { get; set; } = default!;
        public bool TieneAdjunto { get; set; }
        public long? IdAdjunto { get; set; }
    }
}
