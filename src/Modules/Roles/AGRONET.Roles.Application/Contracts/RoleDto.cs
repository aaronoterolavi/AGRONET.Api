using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Roles.Application.Contracts
{
    public sealed class RoleDto
    {
        public int IdRol { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
