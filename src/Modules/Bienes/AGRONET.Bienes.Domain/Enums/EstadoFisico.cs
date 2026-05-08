using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGRONET.Bienes.Domain.Enums;

public static class EstadoFisico
{
    public const string OPERATIVO = "OPERATIVO";
    public const string MANTENIMIENTO = "MANTENIMIENTO";
    public const string BAJA = "BAJA";

    public static readonly List<string> Lista = new() { OPERATIVO, MANTENIMIENTO, BAJA };
}
