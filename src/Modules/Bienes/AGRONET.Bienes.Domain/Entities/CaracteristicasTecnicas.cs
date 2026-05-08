using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Domain.Entities;

public class CaracteristicaTecnica
{
    public int ide_caract_tecnica { get; set; }
    public int ide_bien { get; set; }
    public int? ide_procesador { get; set; }
    public int? num_ram_gb { get; set; }
    public string? txt_capac_disco { get; set; }
    public string flg_disco_solido { get; set; } = "NO";
    public string? txt_tarjeta_video { get; set; }
    public string? txt_pantalla { get; set; }
    public string? txt_direccion_ip { get; set; }
    public string? txt_mac_address { get; set; }
    public DateTime fec_registro { get; set; }
    public DateTime? fec_modificacion { get; set; }
}