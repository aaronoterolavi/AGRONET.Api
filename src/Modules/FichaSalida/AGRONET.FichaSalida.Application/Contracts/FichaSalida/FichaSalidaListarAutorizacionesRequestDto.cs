using AGRONET.FichaSalida.Application.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Contracts.FichaSalida
{
    public class FichaSalidaListarAutorizacionesRequestDto : PagedRequestDto
    {
        public string? EstadoAutorizacion { get; set; } // ej: "01","02","03"
    }
}
