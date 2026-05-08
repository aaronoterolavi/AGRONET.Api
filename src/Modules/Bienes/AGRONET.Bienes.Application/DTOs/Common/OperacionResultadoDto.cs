using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Application.DTOs.Common;

public class OperacionResultadoDto
{
    public int codigo { get; set; }
    public string mensaje { get; set; } = string.Empty;
    public object? data { get; set; }

    public static OperacionResultadoDto Ok(string mensaje = "Operación exitosa", object? data = null)
        => new OperacionResultadoDto { codigo = 1, mensaje = mensaje, data = data };

    public static OperacionResultadoDto Error(string mensaje)
        => new OperacionResultadoDto { codigo = 0, mensaje = mensaje };
}
