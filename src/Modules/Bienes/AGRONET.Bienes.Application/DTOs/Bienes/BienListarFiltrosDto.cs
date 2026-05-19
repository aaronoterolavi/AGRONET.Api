using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Bienes;

public class BienListarFiltrosDto
{
    public string? cod_patrimonial { get; set; }
    public string? txt_nombre { get; set; }
    public int? ide_tipo_bien { get; set; }
    public int? ide_oficina { get; set; }
    public string? est_fisico { get; set; }
    public string? cod_area { get; set; }
    public int page_size { get; set; } = 20;
    public int page_number { get; set; } = 0;
}
