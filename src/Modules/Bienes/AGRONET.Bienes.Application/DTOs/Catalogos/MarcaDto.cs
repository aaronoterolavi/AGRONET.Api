using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Catalogos;

public class MarcaDto
{
    public int ide_marca { get; set; }
    public string? cod_marca { get; set; }
    public string txt_nombre { get; set; } = string.Empty;
}