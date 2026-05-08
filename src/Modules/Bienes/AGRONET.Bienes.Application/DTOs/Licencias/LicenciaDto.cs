using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Licencias;

public class LicenciaDto
{
    public int ide_licencia { get; set; }
    public int? ide_bien { get; set; }
    public int ide_software { get; set; }
    public string software_nombre { get; set; } = string.Empty;
    public string software_version { get; set; } = string.Empty;
    public string txt_num_licencia { get; set; } = string.Empty;
    public DateTime? fec_instalacion { get; set; }
    public DateTime fec_expiracion { get; set; }
    public string? bien_cod_patrimonial { get; set; }
    public string? bien_nombre { get; set; }
    public int dias_restantes { get; set; }
    public string estado_licencia { get; set; } = string.Empty;
    public string? txt_notas { get; set; }
}