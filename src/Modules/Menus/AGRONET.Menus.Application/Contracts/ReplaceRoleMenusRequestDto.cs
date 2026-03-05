using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Menus.Application.Contracts
{
    public sealed class ReplaceRoleMenusRequestDto
    {
        public List<int> MenuIds { get; set; } = new();
    }
}
