using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Domain.Entities;

public class TipoBien
{
    public int ide_tipo_bien { get; set; }
    public string? cod_tipo_bien { get; set; }
    public string txt_nombre { get; set; } = string.Empty;
    public string? txt_descripcion { get; set; }
    public string flg_activo { get; set; } = "SI";
    public DateTime fec_registro { get; set; }
    public DateTime? fec_modificacion { get; set; }
}