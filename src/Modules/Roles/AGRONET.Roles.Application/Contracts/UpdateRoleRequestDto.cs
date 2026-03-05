using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Roles.Application.Contracts
{
    public sealed class UpdateRoleRequestDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
    }
}
