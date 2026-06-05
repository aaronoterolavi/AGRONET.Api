using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Mantenimientos
{
    public class MantenimientoCrearRequestDto
    {
        public int? ide_mantenimiento { get; set; }
        public int ide_bien { get; set; }
        public int ide_tipo_mantenimiento { get; set; }
        public string flg_estado { get; set; } = "P";
        public DateTime fec_mantenimiento { get; set; }
        public string txt_descripcion { get; set; } = string.Empty;
        public string? txt_tecnico_responsable { get; set; }
        public string? txt_observaciones { get; set; }
        public decimal? num_costo { get; set; }
        public DateTime? fec_inicio { get; set; }
        public DateTime? fec_fin { get; set; }
        public string? txt_recomendaciones { get; set; }
        public string? flg_garantia { get; set; }
        public DateTime? fec_proxima_mantenimiento { get; set; }
    }
}
