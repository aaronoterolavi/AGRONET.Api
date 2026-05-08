using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Domain.Entities;

public class Bien
{
    public int ide_bien { get; set; }
    public string cod_patrimonial { get; set; } = string.Empty;
    public string txt_nombre { get; set; } = string.Empty;
    public string? txt_descripcion { get; set; }
    public int ide_tipo_bien { get; set; }
    public int ide_marca { get; set; }
    public string? txt_modelo { get; set; }
    public string? txt_serie { get; set; }
    public DateTime? fec_adquisicion { get; set; }
    public string est_fisico { get; set; } = "OPERATIVO";
    public string cod_area { get; set; } = string.Empty;
    public string? txt_hostname { get; set; }
    public string flg_activo { get; set; } = "SI";
    public string? txt_observaciones { get; set; }
    public DateTime fec_registro { get; set; }
    public DateTime? fec_modificacion { get; set; }
}