using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Catalogos
{
    public class OficinaDto
    {
        public int ide_oficina { get; set; }
        public string cod_oficina { get; set; } = string.Empty;
        public string nom_oficina { get; set; } = string.Empty;
        public string? flg_activo { get; set; }
    }
}
