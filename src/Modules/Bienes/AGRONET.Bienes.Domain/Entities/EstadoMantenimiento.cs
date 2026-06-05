using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Domain.Entities
{
    public class EstadoMantenimiento
    {
        public string ide_estado_mantenimiento { get; set; } = string.Empty;
        public string txt_nombre { get; set; } = string.Empty;
        public string? txt_descripcion { get; set; }
        public string? txt_color { get; set; }
        public string flg_activo { get; set; } = "S";
    }
}
