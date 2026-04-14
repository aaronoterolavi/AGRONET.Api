using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Marcacion.Application.Contracts
{
    public sealed class AperturaMarcacionDto
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public string Observacion { get; set; }
        public string Usuario { get; set; }
    }
}
