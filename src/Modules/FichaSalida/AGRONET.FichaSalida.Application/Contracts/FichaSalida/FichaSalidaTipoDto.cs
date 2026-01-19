using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Contracts.FichaSalida
{
    public sealed class FichaSalidaTipoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = default!;
        public string Descripcion { get; set; } = default!;
        public string Estado { get; set; } = default!;
        public bool FlgJustifica { get; set; }
    }
}
