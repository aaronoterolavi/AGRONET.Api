using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Mantenimientos
{
    public class MantenimientoListarFiltrosDto
    {
        public int? ide_bien { get; set; }
        public int? ide_tipo_mantenimiento { get; set; }
        public string? flg_estado { get; set; }
        public DateTime? fecha_inicio { get; set; }
        public DateTime? fecha_fin { get; set; }
        public string? buscar { get; set; }
        public string? cod_area { get; set; }
        public int? ide_oficina { get; set; }
        public int page_size { get; set; } = 10;
        public int page_number { get; set; } = 0;
    }
}
