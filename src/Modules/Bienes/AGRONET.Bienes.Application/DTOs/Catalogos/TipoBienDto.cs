using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Catalogos;

public class TipoBienDto
{
    public int ide_tipo_bien { get; set; }
    public string? cod_tipo_bien { get; set; }
    public string txt_nombre { get; set; } = string.Empty;
    public string? txt_descripcion { get; set; }
}