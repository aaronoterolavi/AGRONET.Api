using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Catalogos;

public class SoftwareDto
{
    public int ide_software { get; set; }
    public string? cod_software { get; set; }
    public string txt_nombre { get; set; } = string.Empty;
    public string? txt_version { get; set; }
    public string? txt_fabricante { get; set; }
}