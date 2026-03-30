using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Boletas.Domain.Entities
{
    public class Boleta
    {
        public int ICodBoleta { get; set; }
        public string VAnio { get; set; } = string.Empty;
        public string VMes { get; set; } = string.Empty;
        public string VDni { get; set; } = string.Empty;
        public string? VCodArea { get; set; }
        public string? VTipoTrabajador { get; set; }
        public string? VTipoBoleta { get; set; }
        public string? VRutaBoleta { get; set; }
        public string? VCodUnicoBoleta { get; set; }
        public bool BVisto { get; set; }
        public bool BDescargado { get; set; }
        public bool BVistoCorreo { get; set; }
        public bool BCorreoEnviado { get; set; }
    }
}
