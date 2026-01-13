using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Infrastructure.Data.Models
{
    internal sealed class UserRecord
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = default!;
        public string? Correo { get; set; }
        public string DniNorm { get; set; } = default!;
        public string? ApePaterno { get; set; }
        public string? ApeMaterno { get; set; }
        public string? Nombres { get; set; }
        public int IdRol { get; set; }
        public string RolCodigo { get; set; } = default!;
        public string RolNombre { get; set; } = default!;
        public string? CodArea { get; set; }
        public string? CodTipoEmpleado { get; set; }
        public bool Activo { get; set; }
    }
}
