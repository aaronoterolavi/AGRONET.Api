using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Marcacion.Application.Contracts
{
    public sealed class TrabajadorDto
    {
        public string Nombres { get; set; } = "";
        public string NumDocumento { get; set; } = "";
        public string? DscEmailInstitu { get; set; }
        public string? DscCargo { get; set; }
        public string? Observaciones { get; set; }
    }
}
