using AGRONET.Menus.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Menus.Application.Interfaces
{
    public interface IMenuAdminService
    {
        Task<List<MenuAdminDto>> ListarTodosAsync(bool soloActivos, CancellationToken ct);
        Task<List<MenuAdminDto>> ListarArbolAsync(bool soloActivos, CancellationToken ct);
        Task<MenuAdminDto?> ObtenerPorIdAsync(int idMenu, CancellationToken ct);
        Task<int> CrearAsync(CreateMenuRequestDto req, string usuario, CancellationToken ct);
        Task ActualizarAsync(int idMenu, UpdateMenuRequestDto req, string usuario, CancellationToken ct);
        Task CambiarActivoAsync(int idMenu, bool activo, string usuario, CancellationToken ct);
        Task CambiarOrdenAsync(int idMenu, int orden, string usuario, CancellationToken ct);
    }
}
