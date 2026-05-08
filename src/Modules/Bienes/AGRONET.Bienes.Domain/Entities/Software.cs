using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Domain.Entities;

public class Software
{
    public int ide_software { get; set; }
    public string? cod_software { get; set; }
    public string txt_nombre { get; set; } = string.Empty;
    public string? txt_version { get; set; }
    public string? txt_fabricante { get; set; }
    public string flg_licencia_perpetua { get; set; } = "NO";
    public string flg_activo { get; set; } = "SI";
    public DateTime fec_registro { get; set; }
}