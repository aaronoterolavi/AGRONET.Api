using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Mantenimientos
{
    public class MantenimientoDto
    {
        public int ide_mantenimiento { get; set; }
        public int ide_bien { get; set; }
        public string? cod_patrimonial { get; set; }
        public string? nombre_equipo { get; set; }
        public string? txt_modelo { get; set; }
        public string? txt_serie { get; set; }
        public string? sede_nombre { get; set; }
        public string? sub_sede_nombre { get; set; }
        public string? oficina_nombre { get; set; }
        public string? tipo_mantenimiento { get; set; }
        public string? flg_estado { get; set; }
        public string? estado_nombre { get; set; }
        public DateTime fec_mantenimiento { get; set; }
        public string? txt_descripcion { get; set; }
        public string? txt_tecnico_responsable { get; set; }
        public string? txt_observaciones { get; set; }
        public decimal? num_costo { get; set; }
        public DateTime fec_registro { get; set; }

        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
    }
}
