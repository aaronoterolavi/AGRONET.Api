using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Contracts.FichaSalida
{
    public sealed class FichaSalidaTipoDetalleDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = default!;
        public string Cod_FichaSalidaTipo { get; set; } = default!;
        public string? Descripcion { get; set; }
        public string Estado { get; set; } = default!;
        public string? Flg_Justifica { get; set; }
    }
}
