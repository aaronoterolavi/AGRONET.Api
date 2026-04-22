using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Contracts.FichaSalida
{
    public sealed class FichaSalidaActualizarEstadoRequestDto
    {
        public int Id { get; set; }
        public string EstadoAutorizacion { get; set; } = default!;
        public string observacionesVigilancia { get; set; } = default!;

    }
}
