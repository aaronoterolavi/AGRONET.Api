using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Bienes;

public class BienDto
{
    public int ide_bien { get; set; }
    public string cod_patrimonial { get; set; } = string.Empty;
    public string txt_nombre { get; set; } = string.Empty;
    public string? txt_descripcion { get; set; }
    public int ide_tipo_bien { get; set; }
    public string tipo_bien { get; set; } = string.Empty;
    public int ide_marca { get; set; }
    public string marca { get; set; } = string.Empty;
    public string? txt_modelo { get; set; }
    public string? txt_serie { get; set; }
    public DateTime? fec_adquisicion { get; set; }
    public string est_fisico { get; set; } = string.Empty;
    public string cod_area { get; set; } = string.Empty;

    // ========== NUEVAS PROPIEDADES ==========
    public string? sede_nombre { get; set; }      // ← Nombre de la sede
    public string? sub_sede_nombre { get; set; }  // ← Nombre de la sub sede
    public string? licencia_so { get; set; }      // ← Licencia SO (opcional)

    public string? txt_hostname { get; set; }
    public string? procesador { get; set; }
    public int? num_ram_gb { get; set; }
    public string? txt_capac_disco { get; set; }
    public string? tipo_disco { get; set; }
}