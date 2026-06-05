using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Domain.Entities
{
    public class TipoMantenimiento
    {
        public int ide_tipo_mantenimiento { get; set; }
        public string txt_nombre { get; set; } = string.Empty;
        public string? txt_descripcion { get; set; }
        public string? txt_color { get; set; }
        public string flg_activo { get; set; } = "S";
        public DateTime fec_registro { get; set; }
        public DateTime? fec_modificacion { get; set; }
    }
}
