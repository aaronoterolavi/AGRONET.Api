using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Mantenimientos
{
    public class TipoMantenimientoDto
    {
        public int ide_tipo_mantenimiento { get; set; }
        public string txt_nombre { get; set; } = string.Empty;
        public string? txt_descripcion { get; set; }
        public string? txt_color { get; set; }
    }
}
