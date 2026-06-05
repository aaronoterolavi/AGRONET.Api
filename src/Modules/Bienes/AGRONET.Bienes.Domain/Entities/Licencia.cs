using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Domain.Entities;

public class Licencia
{
    public int ide_licencia { get; set; }
    public int? ide_bien { get; set; }
    public int ide_software { get; set; }
    public string txt_num_licencia { get; set; } = string.Empty;
    public DateTime? fec_instalacion { get; set; }
    public DateTime? fec_expiracion { get; set; }
    public string flg_activo { get; set; } = "SI";
    public string? txt_notas { get; set; }
    public DateTime fec_registro { get; set; }
    public DateTime? fec_modificacion { get; set; }
    public string? txt_correo { get; set; }
    public string? txt_contrasena { get; set; }
}