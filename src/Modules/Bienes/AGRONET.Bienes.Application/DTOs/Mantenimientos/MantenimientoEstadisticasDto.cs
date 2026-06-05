using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Mantenimientos
{
    public class MantenimientoEstadisticasDto
    {
        public int total { get; set; }
        public int pendientes { get; set; }
        public int en_proceso { get; set; }
        public int completados { get; set; }
        public decimal costo_total { get; set; }
        public decimal? costo_promedio { get; set; }
    }
}
