using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Licencias;

public class LicenciaListarFiltrosDto
{
    public string? estado_licencia { get; set; }  // VIGENTE, POR VENCER, VENCIDA
    public int? ide_software { get; set; }
    public int? dias_vencer { get; set; }
    public string? buscar { get; set; }
    public int page_size { get; set; } = 20;
    public int page_number { get; set; } = 0;
}
