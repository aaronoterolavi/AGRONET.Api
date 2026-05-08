using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Domain.Entities;

public class Procesador
{
    public int ide_procesador { get; set; }
    public string? cod_procesador { get; set; }
    public string txt_nombre { get; set; } = string.Empty;
    public string? txt_generacion { get; set; }
    public string? txt_fabricante { get; set; }
    public string flg_activo { get; set; } = "SI";
    public DateTime fec_registro { get; set; }
}