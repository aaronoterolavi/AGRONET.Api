using AGRONET.Menus.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Menus.Application.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuDto>> ObtenerArbolPorRolAsync(int idRol, CancellationToken ct);
        Task ReemplazarMenusDeRolAsync(int idRol, List<int> menuIds, string usuario, CancellationToken ct);
    }
}
